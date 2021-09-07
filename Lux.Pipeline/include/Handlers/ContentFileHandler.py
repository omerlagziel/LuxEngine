import os

class ContentFileHandler(object):
	SUPPORTED_EXTENSIONS = []

	def __init__(self, root_input_dir, root_output_dir):
		self.root_input_dir = root_input_dir
		self.root_output_dir = root_output_dir


	def _handle_file(self, input_filepath, output_filepath):
		raise NotImplementedError()

	
	def _post_file_handling(self, content_dir, lux_pipeline_path):
		pass


	@staticmethod
	def change_extension(filepath, new_extension):
		return os.path.splitext(filepath)[0] + new_extension


	@staticmethod
	def get_filename(filepath):
		base = os.path.basename(filepath)
		return os.path.splitext(base)[0]


	def handle(self, filepath):
		(_, extension) = os.path.splitext(filepath)
		if extension not in self.SUPPORTED_EXTENSIONS:
			return

		relative_filepath = os.path.relpath(filepath, self.root_input_dir)
		mirrored_content_filepath = os.path.join(self.root_output_dir, relative_filepath)

		self._handle_file(filepath, mirrored_content_filepath)


	def post_file_handling(self, content_dir, lux_pipeline_path):
		self._post_file_handling(content_dir, lux_pipeline_path)