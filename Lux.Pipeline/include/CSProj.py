from xml.dom import minidom
import os
from pathlib import Path

CONTENT_DIR_NAME = 'Content'
PIPELINE_LABEL = 'AutomatedGamePipeline'
IGNORED_FILES = ['Thumbs.db']

class CSProj:
	def __init__(self, csproj_file_path):
		self.csproj_file_path = csproj_file_path
		self.project_path = csproj_file_path.rpartition('/')[0]
		self.xmldoc = minidom.parse(csproj_file_path)


	def sync_content(self):
		content_dir_path = os.path.join(self.project_path, CONTENT_DIR_NAME)
		was_changed = False
		for path in Path(content_dir_path).rglob('*.*'):
			if path.name.startswith('.') or path.name in IGNORED_FILES:
				continue

			if self._add_asset(path.name, str(path).split(self.project_path, 1)[1]):
				was_changed = True

		if was_changed:
			self.write_changes()


	def add_asset(self, file_path, asset_type):
		filename = file_path.rpartition('/')[2]
		include_path = '{0}/{1}/{2}'.format(CONTENT_DIR_NAME, asset_type, filename)
		was_changed = self._add_asset(file_path, include_path)

		if was_changed:
			self.write_changes()


	def _add_asset(self, file_path, include_path):
		include_path = include_path.strip(os.sep)

		# Find pipeline's item group if it already exists
		itemGroups = self.xmldoc.getElementsByTagName('ItemGroup')
		foundItemGroup = None
		for itemGroup in itemGroups:
			if 'Label' in itemGroup.attributes and itemGroup.attributes['Label'].value.lower() == PIPELINE_LABEL.lower():
				foundItemGroup = itemGroup
				break

		# If pipeline's item group doesn't exist, create it
		if foundItemGroup is None:
			itemGroupElement = self.xmldoc.createElement('ItemGroup')
			itemGroupElement.setAttribute('Label', PIPELINE_LABEL)
			self.xmldoc.childNodes[0].appendChild(itemGroupElement)
			foundItemGroup = itemGroupElement

		# If None element exists already, don't add it
		for noneElement in self.xmldoc.getElementsByTagName('None'):
			if 'Include' in noneElement.attributes:
				include_attr = noneElement.attributes['Include'].value
				if include_attr.lower() == include_path.lower():
					return False

		noneElement = self.xmldoc.createElement('None')
		noneElement.setAttribute('Include', include_path)

		copyToElement = self.xmldoc.createElement('CopyToOutputDirectory')
		copyToElement.appendChild(self.xmldoc.createTextNode('PreserveNewest'))
		noneElement.appendChild(copyToElement)

		foundItemGroup.appendChild(noneElement)

		return True

	def write_changes(self):
		with open(self.csproj_file_path, 'w') as f:
			self.xmldoc.writexml(f)
			f.truncate()