@echo off

FOR /L %%A IN (1,1,400) DO (
echo Round %%A
@start /b cmd /c ADAL_KEYVAULT_EVENTHUBS_ConsoleApp.exe
)
