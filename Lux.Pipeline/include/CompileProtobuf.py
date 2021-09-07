import sys
import subprocess
import os

def main():
    if len(sys.argv) != 4:
        print("Usage: <script> <lux_project_dir> <proto_files_dir> <python_output_dir>")
        return

    lux_project_dir = sys.argv[1]
    protoc_path = '{0}/lib/protoc/bin/protoc'.format(lux_project_dir)
    protoc_include_path = '{0}/lib/protoc/include'.format(lux_project_dir)

    proto_files_dir = sys.argv[2]
    python_output_dir = sys.argv[3]

    # If Windows, adjust protoc filename
    if sys.platform.startswith('win32'):
        protoc_path += '.exe'

    for filename in os.listdir(proto_files_dir):
        if not filename.endswith('.proto'):
            continue

        filepath = '{0}/{1}'.format(proto_files_dir, filename)
        result = subprocess.run([
            protoc_path, 
            '--proto_path=' + proto_files_dir,
            '--proto_path=' + protoc_include_path,
            '--csharp_out=' + proto_files_dir,
            '--python_out=' + python_output_dir,
            filepath])

        # If failed, exit
        if result.returncode != 0:
            return result.returncode

    return 0

if __name__ == "__main__":
    sys.exit(main())