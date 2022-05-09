public class MyApp
{
    public void Main()
    {
        string accessKey = "abc";
        string servicePrincipalKey = "def";
        var oauthClientOptions = new ClientCredentialsOptions()
        {
            ServicePrincipalKey = servicePrincipalKey,
            AccessKey = JsonConvert.DeserializeObject<AccessKey>(accessKey)
        };
    }

    public async Task<ILaserficheRepositoryApiClient> CreateClient()
    {
        //TODO rename Laserfiche.Oauth.Api.Client to Laserfiche.Auth.Api.Client which will contain all auth clients (oauth client cred, oauth auth code, on-prem auth, ...)
        //TODO all of these auth client will return their own implementation of IClientOptions
        //? service app type specific
        string accessKey = "abc";
        string servicePrincipalKey = "def";
        // IClientOptions oauthClientOptions = CreateOauthClientOptions(accessKey, servicePrincipalKey) // part of auth client lib
        //TODO repository api client will depend on auth api client
        //? Should be the same for all apps
        // IRepositoryApiClient repositoryApiClient = RepositoryApiClient.Create(IClientOptions);
        // repositoryApiClient.GetEntry()
        //? Workflow api should be initialized with the same client options as repository client
        // IWorkflowApiClient workflowApiClient = WorkflowApiClient.Create(oauthClientOptions);
        // workflowApiClient.StartWorkflow()
    }
}