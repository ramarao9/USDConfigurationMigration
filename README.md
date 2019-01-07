# USDConfigurationMigration

Refer to the releases section for the zip file.

This tool provides the ability to move a particular USD Configuration from source organization to target organization.

While this might not be beneficial for Organizations that use single configuration this could be useful in couple of places

- More than one configuration exists and would like to migrate a particular configuration when the other configurations are unfinished or currently under development

- Quickly push configurations from DEV to UAT for user acceptance while having the developers continue their development

- Tool handles the export and import process without having the user build the schema file.

- provides detailed information on the outcome of the migration process

- can be used with a PowerShell script to enable Continous Integration


# Limitations

- Does not migrate the customization files similar to the standard Configuration Migration Tool.

- Relies on the name of the configuration to be unique across organizations.



# Roadmap/Backlog

- Include PowerShell Modules for CI/CD




Feedback and contributions welcome! 
