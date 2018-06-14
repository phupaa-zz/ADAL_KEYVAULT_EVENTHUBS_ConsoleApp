# ADAL_KEYVAULT_EVENTHUBS_ConsoleApp
This is a sample Windows console app to demonstrate how to login to Azure Active DIrectory to get secret from Azure KeyVault and send event to Azure Event Hubs

#Before run the code, you need to:
1. Register app on your Azure AD.
2. Setup Azure KayVault and add secret.
3. Setup Azure Event Hubs.
4. Open the sourcecode and update the variables below.

	public static string ClientID = "REPLACE THIS WITH YOUR CLIENT/APP ID HERE";

	public static string ClientSecret = "REPLACE THIS WITH YOUR CLIENT/APP PASSWORD";
	
	public static string SecretName= "REPLACE THIS WITH YOUR SECRET NAME IN KEY VAULT";

	public static string AzureVaultURI = "REPLACE THIS WITH YOUR KEY VAULT URI";

	public const string EhEntityPath = "REPLACE THIS WITH EVENT HUBS ENTITY NAME";


5. Done. This sample code was built by Visual Studio 2017.


*** Tips ***
To simply simulate load and events to Azure Event Hubs, after you have built the .EXE from above sourcecode.  
You can create .BAT file like below to run the .EXE.  Just change 100 to number of instance you want to run at the same time. 


@echo off

FOR /L %%A IN (1,1,100) DO (

echo Round %%A

@start /b cmd /c ADAL_KEYVAULT_EVENTHUBS_ConsoleApp.exe

)
