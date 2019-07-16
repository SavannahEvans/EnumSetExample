# Enum Set Lookup Example

## Files

This program creates a translated resource file from resources.  

enums.txt list all available resources with number identifiers.  

enumSets.txt list all the available sets with number identifiers.  

enumSet\<set\>.txt has the member identifiers for a specific set.  

translatedEnums.txt will be generated from all these resources with the number identifiers replaced with the true values.  

## Run

It must be noted that the search function does not use the updated file. You will have to run it twice to first update the file, then to use it. This is due to the resources being compiled before runtime, and they will not update.  

First run: "dotnet run -u" to update the translatedEnum.txt file  
Second use search function: "dotnet run -s \<set\> -m \<member\>
