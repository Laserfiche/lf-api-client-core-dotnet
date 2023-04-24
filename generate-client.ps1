# Generate the client
nswag run apiserver-nswag.json
nswag run oauth-nswag.json

# Remove long namespaces from class names in the generated client
$client_filepaths = @("./src/APIServer/TokenClient.cs", "src/OAuth/OAuthClient.cs")
foreach ( $client_filepath in $client_filepaths )
{
    (Get-Content $client_filepath) `
        -replace "System\.CodeDom\.Compiler\.","" `
        -replace "System\.Collections\.Generic\.","" `
        -replace "System\.Globalization\.","" `
        -replace "System\.IO\.","" `
        -replace "System\.Linq\.","" `
        -replace "System\.Net\.Http\.Headers\.","" `
        -replace "System\.Net\.Http\.(?![\w\.]+;)","" `
        -replace "System\.Runtime\.Serialization\.","" `
        -replace "System\.Reflection\.","" `
        -replace "System\.Text\.","" `
        -replace "System\.Threading\.Tasks\.","" `
        -replace "System\.Threading\.CancellationToken","CancellationToken" `
        -replace "System\.(?![\w\.]+;)","" `
        | Out-File $client_filepath
}
