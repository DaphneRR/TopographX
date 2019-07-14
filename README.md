# TopographX

## Overview
Welcome to TopographX, a small, plug-and-play software for iEEG data visualization, designed and programmed in collaboration by Antoine Dieulesaint and Daphné Rimsky-Robert.\
This software comes in an executable build you can download from here, and allows for the creation of specific web-pages to display your data and make it globally available. Currently, the software has been tested and validated for Windows. Linux and MacOS builds are available but couldn't be tested.

So far, data can be plotted in MNI-space using the ICBM-152 template, but we plan on making the use of different brain-meshes possible in the future.

To start using TopographX, you only need a single csv file formatted as described below, which can readily be loaded in the software interface. Several options for plotting and better visualization are available in-software. Specific parameter configurations can be saved and loaded.

Below are specific instructions and details for use of TopographX. Besides the download, file and data format sections, most options in TopographX should be reasonably intuitive. You can initially skip these, and come back to them if something isn't clear, or you would like to see what is possible in TopographX.
## Download
...
## Acceptable file formats
Your data must be formatted in a csv-file, with either a separator "," or ";". In the first case, decimals should be indicated with ".", while in the second, they should be indicated with ",". Using the "," separator syntax is the default in TopographX. In order to use the other format, you need to go to "Options" and input the "csv files separating character" as ";".

### Separator "," (default):
1.1,2.2,3.3\
4.4,5.5,6.6
### Separator ";":
1,1;2,2;3,3\
4,4;5,5;6,6

## Data formatting
### Header (first line)
The first line of data should contain in order the total number of electrodes in-file, the text you wish to display next to your time-stamps (e.g. s or ms), followed by the time-stamps of the series you wish to plot.
### Rest of the file (body)
Your file body should be organized by columns:
The first column should refer to individual electrode number. These should be unique. You can choose to have this range from 1 to "total number of electrodes" for example. These have to be integers (no decimal!).\
The second column should contain electrode labels. These are optional, and can be replaced by a column of 0 if you wish. These can be text or numbers.\
The third column should contain participant identifiers for each electrode. These are optional, and can be replaced by a column of 0 if you wish. These can be text or numbers.\
Columns 4 to 6 should respectively indicate the x, y and z coordinates of each electrode, in MNI space (for now). These have to be numbers.
The rest of the columns should correspond to the data you wish to visualize, at the time-stamps indicated by the header. These have to be numbers.
#### example
2,s,0,0.5,1,1.5\
1,Heschl,Tom,x1,y1,z1,0,78.43,80.22,12.1\
2,Tsup,Tom,x2,y2,z2,0,33,54.211,0

This dataset contains 2 electrodes labeled Heschl and Tsup, located respectively at (x1,y1,z1) and (x2,y2,z2). Both electrodes come from Patient Tom. The associated data is shown starting from after z1 and z2, and were sampled at timestamps 0, 0.5, 1 and 1.5 s.

## Data Manager
### Datasets
Once you have your data formatted and saved in a csv file, you can load it in TopographX.\
Start the software using TopographX.exe, and go in "Data Manager". You can load a dataset by clicking on "Add Dataset" in the "Datasets". Navigate to the relevant folder, and double-click on your file or click once and use the load icon located at the bottom-right corner. If the data is appropriately formatted, it should be displayed in the "Datasets" tab. If not, it will not load and you will remain in the file explorer. You can add more than one dataset at a time, and these will have labels in order: Set1, Set2 etc.
### Formula
You can perform basic data transformations in this tab, to avoid having to recalculate entire datasets outside the software if the current ones are not *exactly* what you wanted.\
With one dataset, you could indicate "1/Set1" to display the inverse of the loaded data (without the ""). With more than one dataset loaded, you can indicate which one you would like to display by entering "Set1" or "Set2" etc. You can also make basic calculations involving more than one dataset, e.g. "Set1 - Set2".\
This option was added for convenience, however TopographX was not designed for data processing. Complex operations may take a very long time to compute. Please use this option with parsimony.
Recognized operations in the "Formula" tab come from [MathParser](https://github.com/mariuszgromada/MathParser.org-mXparser#built-in-tokens).

### Colors
The default color for electrodes is used when the software couldn't tell what color to attribute to an electrode. This usually indicates a bug, either in the Formula, in the data, or the software (hopefully not).\
Two major options are available for color-coding.\
"Threshold" allows for plotting all data below a certain threshold in one color, and all data above it another. \
"Colormap" allows you to load colormaps into TopographX as described in the "Datasets" section. Once loaded, TopographX will map colors from the colormap from first to last onto the values within the dataset from the minimal value to the maximal value. Mapping from last to first can be achieved using the "Invert" box. Adding a colorbar on the display is currently not possible, but the option will be added soon.
"Visible" buttons, when unchecked, hide electrodes of the chosen category.\
Bounds for the colormap and threshold values can be changed in the input fields on the right of the "Data Manager" button. 

## Config
You can save, load and copy configurations for TopographX from this menu. This will save *everything*, including the current dataset, mesh position, colors, etc. for loading at a later time, or making an online visualization set.\ With large datasets, saving can take a while. With 1135 electrodes and 24 time steps, this takes about 15s on a somewhat recent laptop. 

## Options
Here, you can change various properties of the TopographX environment, incuding the background color, whether the x,y,z axes are shown, the mesh color, transparency and glossiness, electrode size, and csv file separating character.

## Mesh display
The "Left" and "Right" tickboxes allow for hiding or showing hemishperes individually. Icons located to the right of these boxes allow for showing the hemispheres facing, or side-by-side. The camera can be moved by "drag and drop", meaning that you should click anywhere, hold, and move the cursor on screen to change the camera angle. Releasing the click suffices to stop moving the camera. 

## Navigating through the time series
Using the left and right arrow keys on your keyboard should help you navigate the time series you have displayed. The bottom bar indicates what element of the series is being displayed on a time axis, with its stamp shown on the bottom right. Dragging the cursor on the bar will also work.\
A "play" button is available to have all images in the series be shown in order from the starting point of your choosing. Display speed can be regulated using the bottom-left slider. A "back to start" button is available next to the "play" button.

## Image and Video capture
This feature is not currently available (sadly), but will be added soon. For now, image capture can be achieved by "Print Screen" on a full-screen display of TopographX.\
Otherwise, we recommend using the Fraps software which allows for high-quality image and video capture.

## contact
Please, do not hesitate to contact us at drr@tuta.io or a.dieulesaint@gmail.com if you wish to contribute to the project (coded in Unity, C#), report a bug, give us feedback or suggestions for future releases. 
