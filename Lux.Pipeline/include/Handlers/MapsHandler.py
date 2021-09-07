from Handlers.ContentFileHandler import ContentFileHandler
import json

class MapsHandler(ContentFileHandler):
    SUPPORTED_EXTENSIONS = ['.json']

    def __init__(self, root_input_dir, root_output_dir):
        ContentFileHandler.__init__(self, root_input_dir, root_output_dir)

    def _handle_file(self, input_filepath, output_filepath):
        print(input_filepath)
        with open(input_filepath, 'r') as f:
            json_obj = json.load(f)

        if 'ogmoVersion' not in json_obj:
            return

        with open(output_filepath, 'w') as f:
            json.dump(json_obj, f)
            f.truncate()
