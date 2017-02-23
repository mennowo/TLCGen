This directory is meant to contain files that may be needed
to generate a working controller, but are not allowed to be
distributed open source along with the rest of TLCGen.

Files placed here will be copied to the destination folder
for the generation process, if they do not already exist,
and the controller meets the specified condition, as
described below.

The first line in a file must contain a condition, specifying
when the files should be copied. This line will be removed
by TLCGen. This line is mandatory!

Options are:
CONDITION=ALWAYS
CONDITION=OV
CONDITION=SYNC
CONDITION=NALOPEN
CONDITION=RGV

Use only one condition, and place no other text on the first
line of the file.