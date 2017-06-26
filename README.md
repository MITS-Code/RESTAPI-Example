# REST API Example
This repository contains a C# Visual Studio Console Project implementation of some parts of the Zoho Desk Rest API Interface.

It uses:
- Linq
- RestSharp
- JSON Objects and their serializers and deserializers
- **private.config file for obfuscating private login credentials**

## How to use
---------------
**This solution is not straight forward to use. For the .exe to run correctly.** 
1. you will need to have:
    1. RESTAPIExample.exe
    1. RestSharp.dll
    1. private.config

    All in the __same directory__.
    The application will not start if this is not the case.

1. Once you have all the files required, you will need to edit the private.config file.
An example is provided in the root of the project, with the .csproj and .cs files. This file is **NOT IN THE CORRECT PLACE** and only serves as a template to what your private.config file should look like.

    An example of **_private.config_** will be provided below:
    ```xml
    <?xml version="1.0" encoding="utf-8" ?>
    <configuration>
    <appSettings>
        <add key="username" value="youremailaddress@gmail.com"/>
        <add key="password" value="yourpassword"/>
        <add key="orgId" value="yourorganisationID"/>
        <add key="Authorization" value="yourAuthToken"/>
    </appSettings>
    </configuration>
    ```

## Additional Tools
--------------
1. Postman - https://www.getpostman.com/ - Useful for testing and developing REST API Calls and Returns
1. JSON to C# - http://json2csharp.com/ - Useful for turning the returned JSON into C# objects. Does what it says on the tin.
1. Visual Studio 2017 - Paste Special -> Paste as JSON Objects - Same as above