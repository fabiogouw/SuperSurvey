using Amazon;
using Amazon.RDS.Util;
using Microsoft.Extensions.Logging;

namespace SuperSurvey.Adapters
{
    public class AWSMySQLPollRepository : MySQLPollRepository
    {
        public AWSMySQLPollRepository(ILogger<AWSMySQLPollRepository> logger, 
            string region, 
            string dbEndpoint, 
            string dbUser, 
            string connectionStringTemplate) : base(BuildConnectionString(logger, 
                region, 
                dbEndpoint, 
                dbUser, 
                connectionStringTemplate))
        {
        }

        private static string BuildConnectionString(ILogger<AWSMySQLPollRepository> logger,
            string region,
            string dbEndpoint,
            string dbUser,
            string connectionStringTemplate)
        {
            logger.LogTrace($"MySql connection -> Region:{region}, Endpoint:{dbEndpoint}, User:{dbUser}, Template:{connectionStringTemplate}");
            var pwd = RDSAuthTokenGenerator.GenerateAuthToken(RegionEndpoint.GetBySystemName(region), dbEndpoint, 3306, dbUser);
            return connectionStringTemplate.Replace("{pwd}", pwd);
        }
    }
}
