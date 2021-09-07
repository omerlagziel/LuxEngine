import subprocess
import os

class Atlas(object):
    def __init__(self, output_path_no_extension, input_images, lux_pipeline_path):
        self.output_path = output_path_no_extension
        self.input_images = input_images
        self.crunch_path = os.path.join(lux_pipeline_path, 'lib/crunch/bin/crunch')

        if os.name == 'nt':
            self.crunch_path += '.exe'

    
    def set_input_png_path_list(self, arr):
        self.input_png_list = arr

    
    def generate_atlas(self):
        cmdline = ' '.join([
            self.crunch_path,
            self.output_path,
            '"{}"'.format(','.join(self.input_images)),
            '--premultiply',
            '--json',
            #'--trim',
            #'--unique',
            '--size4096',
            '--pad1',
        ])

        result = subprocess.run(cmdline, shell=True)

        if result.returncode != 0:
            raise Exception("Atlas failed to export")
