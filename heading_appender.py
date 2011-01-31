""" This script prepends a heading (read from header.txt) to all .cs files.
    It searches for all the .cs files including *.designer.cs recursively
    starting at the location of this script.
    This script accepts two command line arguments:
    -l, --list : lists all files that are going to be processed
    -a, --append : Appends the contents of header.txt to each *.cs files
"""
# Author: Ashok Gelal
# Version: 0.0.1
# And just for historical reason - this is my first Python script

import os, glob, fnmatch
from optparse import OptionParser

""" Lists the files that are going to be affected by this script """
def list_file(pattern):
	print("\nFollowing files will be affected")
	for cs_file in files:
		print(cs_file)

""" Processes all the files recrusively """
def process_file(files):
	count = 0
	try:
		with open('header.txt', 'r') as header_file:
			header_data = header_file.read()
			for cs_file in files:
				print('Processing: ' + cs_file)
				try:
					with open(cs_file, 'r+') as output_file:
						old_data = output_file.read()
						output_file.seek(0)
						output_file.write(header_data + '\n\n' + old_data)
						count = count+1
				except IOError as err:
					print('Error while appending header' + str(err))
			
		print('\nFinished! Total Files Processed: ' + str(count))
	except IOError as err:
		print('Error while reading header.txt: ' + str(err))
	

# parsing logic
usage = "usage: %prog [options]"
parser = OptionParser(usage)
parser.add_option("-l", "--list", dest="do_list", help="Lists the files that are going to be processed", default=False, action='store_true')
parser.add_option("-a", "--append", dest="do_append", help="Appends the contents of header.txt to each *.cs files", default=False, action='store_true')
(options, args) = parser.parse_args()

files = []
# search for all the *.cs files recursively
for root, dirs, filenames in os.walk(os.path.abspath(os.curdir)):
	for filename in fnmatch.filter(filenames, '*.cs'):
		files.append(os.path.join(root, filename))

if options.do_list:
	list_file(files)
elif options.do_append:
	process_file(files)
else:
	parser.print_help()

