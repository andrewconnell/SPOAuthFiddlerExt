using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

namespace SPOAuthFiddlerExt
{
    public class JsonWebToken
    {
        /// <summary>
        /// The decoded and formatted header of the JWT token
        /// </summary>
        public string Header { get; set; }
        /// <summary>
        /// The decoded and formatted body of the JWT token
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// The decoded signature of the JWT token  
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// A dictionary of the claims from the decoded JWT header
        /// </summary>
        public IDictionary<string,object> HeaderClaims { get; set; }
        /// <summary>
        /// A dictionary of the claims from the decoded JWT body
        /// </summary>
        public IDictionary<string,object> BodyClaims { get; set; }

        public string IssuerID { get; set; }

        public string ClientID { get; set; }

        public string UserID { get; set; }

        public string TenantID { get; set; }

        public string HostName { get; set; }

        

        /// <summary>
        /// Given a JWT token, decodes it and splits the parts into a
        /// JsonWebToken object.
        /// </summary>
        /// <param name="token">String value formatted according to JWT spec</param>
        /// <returns>Decoded JsonWebToken object</returns>
        public static JsonWebToken GetTokenFromString(string token)
        {
            if(string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("Token is null or empty", "token");
            }
            JsonWebToken ret = new JsonWebToken();

            string[] decodedToken = Decode(token);
                        
            ret.Header = JsonWebToken.GetPrettyPrintedJson(decodedToken[0]);
            ret.Body = JsonWebToken.GetPrettyPrintedJson(decodedToken[1]);
            //signature is not JSON, so output it as the raw value
            ret.Signature = decodedToken[2];
                        
            ret.HeaderClaims = JsonConvert.DeserializeObject<IDictionary<string,object>>(ret.Header);
            ret.BodyClaims = JsonConvert.DeserializeObject<IDictionary<string, object>>(ret.Body);
            
            if (ret.BodyClaims.ContainsKey("actortoken"))
            {
                //Actor token is an encoded inner token.  Decode it and get the parts.
                JsonWebToken actorToken = GetTokenFromString(ret.BodyClaims["actortoken"].ToString());
                ret.BodyClaims["actortoken"] = actorToken;                
            }

            string audience = ret.BodyClaims["aud"].ToString();

            ret.HostName = string.Empty;
            ret.TenantID = string.Empty;
            ret.ClientID = string.Empty;
            ret.UserID = string.Empty;
            ret.IssuerID = string.Empty;

            bool isHighTrust = false;
            bool isLowTrust = false;
            bool isContext = false;

            if(ret.BodyClaims.ContainsKey("appctx") && ret.BodyClaims.ContainsKey("appctxsender") && ret.BodyClaims.ContainsKey("isbrowserhostedapp"))
            {
                //This is a SharePoint context token
                isContext = true;                
            }
            else if (audience.StartsWith("00000003-0000-0ff1-ce00-000000000000/"))
            {
                //This is a SharePoint access token.  Now figure out if it's high or low trust
                if(ret.BodyClaims.ContainsKey("actortoken"))
                {
                    //This is a high trust token
                    isHighTrust = true;                    
                }
                else
                {
                    isLowTrust = true;                    
                }
            }
            else
            {
                //This is a JWT token but not one from SharePoint
                //no-op
            }
            
            if(isHighTrust || isLowTrust || isContext)
            {
                ret.HostName = audience.Substring(audience.IndexOf("/") + 1, audience.IndexOf("@") - audience.IndexOf("/") - 1);
                ret.TenantID = audience.Substring(audience.IndexOf("@") + 1, audience.Length - audience.IndexOf("@") - 1);
                if (ret.BodyClaims.ContainsKey("nameid"))
                {
                    ret.UserID = ret.BodyClaims["nameid"].ToString();
                }
            }

            if(isHighTrust)
            {
                //For high-trust apps, the issuerID is in the actortoken
                ret.IssuerID = ((JsonWebToken)ret.BodyClaims["actortoken"]).BodyClaims["iss"].ToString();

                //For high-trust apps, the nameid in the actortoken is the app's client ID
                string clientID = ((JsonWebToken)ret.BodyClaims["actortoken"]).BodyClaims["nameid"].ToString();
                ret.ClientID = clientID.Substring(0, clientID.IndexOf("@"));
            }

            if (isLowTrust)
            {
                //For low-trust apps, the issuer is Azure ACS.
                ret.IssuerID = ret.BodyClaims["iss"].ToString();

                if (ret.BodyClaims.ContainsKey("actor"))
                {
                    string clientID = ret.BodyClaims["actor"].ToString();
                    if (clientID.Contains("@"))
                    {
                        ret.ClientID = clientID.Substring(0, clientID.IndexOf("@"));
                    }
                    else
                    {
                        ret.ClientID = clientID;
                    }
                }
                else
                {
                    if(ret.BodyClaims.ContainsKey("nameid"))
                    { 
                        //App-only low-trust
                        string clientID = ret.BodyClaims["nameid"].ToString();
                        ret.ClientID = clientID.Substring(0, clientID.IndexOf("@"));
                    }
                }
            }
            if(isContext)
            {
                ret.IssuerID = ret.BodyClaims["iss"].ToString();
                ret.ClientID = audience.Substring(0, audience.IndexOf("/") - 1);
               
            }

            return ret;            
        }




        /// <summary>
        /// Splits a JWT token into its parts and decodes each part
        /// </summary>
        /// <param name="jwtToken">The JWT token to decode</param>
        /// <returns>String array containing the header, body, and signature</returns>
        private static string[] Decode(string jwtToken)
        {
            var parts = jwtToken.Split('.');
            //A JWT token *should* be 3 parts: header, body, and signature.
            //We will accept unsigned tokens, where there is only 2 parts.
            if (parts.Length < 2)
            {
                throw new ArgumentException("Invalid JWT token, missing required '.' characters.", jwtToken);
            }
            string[] ret = new string[3];

            var header = parts[0];
            var body = parts[1];

            var decodedHeader = Base64UrlDecoder.Decode(header);
            ret[0] = decodedHeader;

            string decodedBody = Base64UrlDecoder.Decode(body);
            ret[1] = decodedBody;

            if(parts.Length == 3)
            {                
                string decodedSignature = Base64UrlDecoder.Decode(parts[2]);
                ret[2] = decodedSignature;
            }
            else
            {
                ret[2] = string.Empty;
            }
            
            return ret;
        }


        /// <summary>
        /// Gets a formatted and indented representation of the JSON string
        /// </summary>
        /// <param name="json">The JSON to format</param>
        /// <returns>A formatted and indented representation</returns>
        public static string GetPrettyPrintedJson(string json)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }

        /// <summary>
        /// Returns a string representation of the object, formatted as JSON
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {        
            string body = this.Body;

            if (this.BodyClaims.ContainsKey("actortoken"))
            {
                //Replace the encoded actortoken with the decoded body value
                JsonWebToken actorToken = this.BodyClaims["actortoken"] as JsonWebToken;

                //Find the start of the actortoken claim value
                int index = body.IndexOf("actortoken\": ") + "actortoken\": ".Length;
                //Find the length of the actortoken claim value
                int length = body.IndexOf('\n', index) - index;

                //Increase indentation so it looks like a nested object
                string actorBody = actorToken.Body.Replace("\n ", "\n    ");
                
                //Increase indentation of the brackets
                actorBody = actorBody.Replace("{", "\n  {");
                actorBody = actorBody.Replace("}","  }");
                
                //Replace the actortoken claim value with the decoded and formatted token body
                body = body.Replace(body.Substring(index, length), actorBody);

            }
            return string.Format("{0}\n{1}", this.Header, body);        
        }
    }

}


