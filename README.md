# Scampalyze
Scampalyze is a simple tool to check characteristics of a an XML formatted SCAMP (*.form usually)
## Example Command Line
	with output
...
C:\Code\Scampalyze\Scampalyze\bin\Debug>Scampalyze.exe count:"element" filepath:BWH_CHF_24HRCALL.2.0.form
Count of <element>: 13
...
## Current parameters
Command | Description of parameters
--------|--------------------------
filepath: | path to xml file to check
countelement: | command to count a particular element (string that is a valid element tag)
