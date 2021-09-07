import os
import sys
import subprocess
import errno
import json
from Protobuf import Sprite_pb2
from google.protobuf.json_format import MessageToJson, Parse
from Handlers.ContentFileHandler import ContentFileHandler
from Atlas import Atlas

TEXTURE_PADDING = 1

class AsepriteHandler(ContentFileHandler):
	SUPPORTED_EXTENSIONS = ['.aseprite', '.ase']

	def __init__(self, root_input_dir, root_output_dir):
		ContentFileHandler.__init__(self, root_input_dir, root_output_dir)
		self.json_output_paths = {}
		self.temp_png_output_paths = set()


	def _handle_file(self, input_filepath, output_filepath):
		# Export aseprite to png and get the json descriptor
		png_output_path = ContentFileHandler.change_extension(input_filepath, '.png')
		aseprite_json = self._export_aseprite(input_filepath, png_output_path)

		# Add dir to list for when we create the atlas
		self.temp_png_output_paths.add(png_output_path)

		# TODO: Add the texture to an atlas and provide the atlas' image path instead of png_path
		filename = ContentFileHandler.get_filename(output_filepath)
		game_json = self._aseprite_json_to_game_json(filename, aseprite_json)

		json_output_path = ContentFileHandler.change_extension(output_filepath, '.json')

		try:
			os.makedirs(os.path.dirname(json_output_path))
		except OSError as e:
			if e.errno != errno.EEXIST:
				raise

		# Create game json file in content directory
		with open(json_output_path, 'w') as f:
			json.dump(game_json, f)
			f.truncate()

		# Add to list to later update the texture path to the atlas one
		self.json_output_paths[filename] = json_output_path


	def _post_file_handling(self, content_dir, lux_pipeline_path):
		output_path_no_extension = os.path.join(content_dir, "Textures", "atlas")
		atlas = Atlas(output_path_no_extension, self.temp_png_output_paths, lux_pipeline_path)
		atlas.generate_atlas()

		atlas_json_path = output_path_no_extension + '.json'
		with open(atlas_json_path, 'r') as f:
			atlas_json = json.load(f)

		for texture in atlas_json['textures']:
			for image in texture['images']:
				sprite_json_path = self.json_output_paths[image['n']]
				with open(sprite_json_path, 'r') as f:
					sprite_json = json.load(f)
				
				sprite = Sprite_pb2.Sprite()
				Parse(json.dumps(sprite_json), sprite)

				sprite.TextureName = texture['name']

				for animation_key in sprite.Animations:
					for frame in sprite.Animations[animation_key].Frames:
						#frame.Width = image['w']
						#frame.Height = image['h']
						frame.TexturePositionX += image['x']
						frame.TexturePositionY += image['y']

				with open(sprite_json_path, 'w', encoding='utf-8') as f:
					json_obj = json.loads(MessageToJson(sprite, preserving_proto_field_name=True, including_default_value_fields=True))
					json.dump(json_obj, f, ensure_ascii=False, indent=4)
					f.truncate()

		# Remove atlas.json
		#os.remove(atlas_json_path)

		#for temp_png in self.temp_png_output_paths:
			#os.remove(temp_png)


	def _export_aseprite(self, aseprite_filepath, dest_png_path, dest_json_path=None):
		temp = False
		if dest_json_path is None:
			temp = True
			dest_json_path = ContentFileHandler.change_extension(dest_png_path, '.tmp.json')

		result = subprocess.run([
			self._get_program_path(),
			'-b', aseprite_filepath, 
			'--sheet', dest_png_path, 
			'--data', dest_json_path, 
			'--list-tags', 
			'--shape-padding', '${TEXTURE_PADDING}',
			'--border-padding', '${TEXTURE_PADDING}',
			'--sheet-width', '2048',
		])

		if result.returncode != 0:
			raise Exception("Aseprite failed to export")

		aseprite_json = AsepriteJSON(dest_json_path)

		if temp:
			os.remove(dest_json_path)

		return aseprite_json


	def _aseprite_json_to_game_json(self, texture_name, aseprite_json):
		sprite = Sprite_pb2.Sprite()
		sprite.TextureName = texture_name

		# For each frame tag
		frametags = aseprite_json.get_frametags()

		if len(frametags) > 0:
			sprite.DefaultAnimationName = frametags[0]['name']

		for current_tag in frametags:
			frames = []

			# Construct frames
			for current_frame in aseprite_json.get_frames_by_frametag(current_tag):
				frames.append(Sprite_pb2.AnimationFrame(
					Width = current_frame['frame']['w'],
					Height = current_frame['frame']['h'],
					TexturePositionX = current_frame['frame']['x'],
					TexturePositionY = current_frame['frame']['y'],
					SpriteDepth = Sprite_pb2.SpriteDepth.BehindCharacter,
					Duration = current_frame['duration']
				))

			# Add animation to sprite
			sprite.Animations[current_tag['name']].Frames.extend(frames)
			sprite.Animations[current_tag['name']].IndexStart = current_tag['from']
			sprite.Animations[current_tag['name']].IndexEnd = current_tag['to']


		json_str = MessageToJson(sprite, preserving_proto_field_name=True, including_default_value_fields=True)

		return json.loads(json_str)


	def _get_program_path(self):
		program_path = r"C:\Program Files (x86)\Aseprite\Aseprite.exe"

		# If Windows
		if sys.platform.startswith('win32'):
			if not os.path.isfile(program_path):
				program_path = r"C:\Program Files\Aseprite\Aseprite.exe"

		# If MacOS
		if sys.platform.startswith('darwin'):
			program_path = '/Applications/Aseprite.app/Contents/MacOS/aseprite'

		return program_path


class AsepriteJSON:
	def __init__(self, path):
		if not path.endswith('.json'):
			raise Exception('AsepriteJSON got a non json file')

		self.json = None
		with open(path, 'r') as f:
			self.json = json.load(f)


	def get_frametags(self):
		return self.json['meta']['frameTags']


	def get_frames_by_frametag(self, frametag):
		frames = []
		for i in range(frametag['from'], frametag['to'] + 1):
			frames.append(self._get_frame_by_index(i))

		return frames


	# TODO: Make this more reliable by looking for {i}.asperite
	# instead of by index which is unreliable for json objects
	def _get_frame_by_index(self, requestedIndex):
		i = 0
		for frameName in self.json['frames']:
			if requestedIndex == i:
				return self.json['frames'][frameName]
			i += 1