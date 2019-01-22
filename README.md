# USDConfigurationMigration

Refer to the [releases](https://github.com/ramarao9/USDConfigurationMigration/releases) section for the compiled zip file.

This tool provides the ability to move a particular USD Configuration from source organization to target organization.

While this might not be beneficial for Organizations that use single configuration this could be useful in couple of places

- More than one configuration exists and would like to migrate a particular configuration when the other configurations are unfinished or currently under development

- Quickly push configurations from DEV to UAT for user acceptance while having the developers continue their development

- Tool handles the export and import process without having the user build the schema file.

- Provides detailed information on the outcome of the migration process

- Can be used with a PowerShell script to enable Continous Integration

# Using the Tool

After the release is extracted, open the USDConfigurationMigration.WPF which would open up the below screen


![](https://ramarao.blob.core.windows.net/usdconfigurationmigration/ConfigScreen.png)
   
- Select Source :- Specify the Source CRM Organization to export the USD configuration data. The data is stored in data.xml file and could be used for reference purpose.
- Select Target :- Specify the Target CRM Organization to import the exported configuration data.
- Configuration :- The configuration that needs to be migrated from Source to target. 



### Login Screen

To add a new organization, specify the below information

  ![](https://ramarao.blob.core.windows.net/usdconfigurationmigration/LoginScreen.png)

- Organization URL :- The Url of the Dynamics 365 org. You might have to append the unique org name if there are more than one org in the tenant.

- Save Connection :- Saves the connection on the windows credential securely to easily connect in the future.

  ![](https://ramarao.blob.core.windows.net/usdconfigurationmigration/SavedConnections.png)


### Checking the Job Details

After the migration is complete, you could review the logs to  see if there were any failures encountered during the process and also a summary of the migration.

Below is a sample job summary which includes the different metrics

![](https://ramarao.blob.core.windows.net/usdconfigurationmigration/SampleJobSummary.png)


# Limitations

- Does not migrate the customization files similar to the standard Configuration Migration Tool.

- Relies on the name of the configuration to be unique across organizations.



# Roadmap/Backlog

- Include PowerShell Modules for CI/CD




Feedback and contributions welcome! 

**No warranty expressed or implied - use at your own risk!**
