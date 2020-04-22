cd StartUp
echo init >> log.txt 
ds31_pro_rc41.exe ds31_lisit_r01 /verysilent >> log.txt 
copy supplier_info.ini.p7m "%ProgramFiles(x86)%\CompEd\DigitalSign Pro 3.1\supplier_info.ini.p7m"
echo end >> log.txt 