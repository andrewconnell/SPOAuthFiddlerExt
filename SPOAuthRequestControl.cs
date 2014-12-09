using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using Newtonsoft.Json;




namespace SPOAuthFiddlerExt
{
    public partial class SPOAuthRequestControl : UserControl
    {
        public SPOAuthRequestControl()
        {
            InitializeComponent();

        }

        /// <summary>
        /// The HTTP headers that are being set for the control
        /// </summary>
        public Dictionary<string, string> HTTPHeaders
        {
            set
            {
                txtContext.Text = string.Empty;
#if LOCAL_DEBUG
                //This section only used for debugging.  Remove the LOCAL_DEBUG
                //directive and recompile for normal operation.

                //TODO:  Pull a token from the tests

                UpdateUI(token);

#else
                foreach (string key in value.Keys)
                {
                    if (key == "Authorization")
                    {
                        //Access token
                        string accessToken = value[key].Replace("Bearer ", string.Empty);

                        UpdateUI(accessToken);

                    }
                }
#endif

            }
        }

        /// <summary>
        /// The HTTP body that is being set for the control
        /// </summary>
        public byte[] HTTPBody
        {
            set
            {
                bool found = false;
                if (null != value)
                {
                    string ret = System.Text.Encoding.UTF8.GetString(value);

                    //Context token may be sent as a querystring with these names, but this is not
                    //recommended practice and is not implemented here as a result.  Only POST values
                    //are processed.
                    string[] formParameters = ret.Split('&');
                    string[] paramNames = { "AppContext", "AppContextToken", "AccessToken", "SPAppToken" };
                    
                    foreach (string valuePair in formParameters)
                    {
                        string[] formParameter = valuePair.Split('=');
                        foreach (string paramName in paramNames)
                        {
                            if (formParameter[0] == paramName)
                            {
                                UpdateUI(formParameter[1]);

                                found = true;
                                break;
                            }
                        }
                        if (found)
                            break;
                    }
                }

            }
        }

        private void UpdateUI(string tokenString)
        {
            JsonWebToken token = JsonWebToken.GetTokenFromString(tokenString);

            txtContext.Text = token.ToString();

            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            dataGridView3.Rows.Clear();

            PopulateClaimsGrid(token.BodyClaims, dataGridView1);
            PopulatePropertyGrid(token);
        }


        public void Clear()
        {
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            txtContext.Text = string.Empty;

        }

        private void PopulatePropertyGrid(JsonWebToken token)
        {
            dataGridView3.Rows.Add("ClientID", token.ClientID);
            dataGridView3.Rows.Add("TenantID", token.TenantID);
            dataGridView3.Rows.Add("HostName", token.HostName);
            dataGridView3.Rows.Add("IssuerID", token.IssuerID);
            dataGridView3.Rows.Add("UserID", token.UserID);


        }
        private void PopulateClaimsGrid(IDictionary<string, object> dictionary, DataGridView grid)
        {            
            foreach (string key in dictionary.Keys)
            {
                switch (key)
                {
                    case "nbf":
                    case "exp":
                        double d = double.Parse(dictionary[key].ToString());
                        DateTime dt = new DateTime(1970, 1, 1).AddSeconds(d);
                        grid.Rows.Add(key, dt);
                        break;
                    case "actortoken":
                        //actortoken is a JsonWebToken type
                        JsonWebToken token = (JsonWebToken)dictionary[key];
                        string formatted = string.Format("{0}.{1}.{2}", token.Header, token.Body, token.Signature);

                        grid.Rows.Add(key, formatted);
                        
                        //Show the actor token in the second data grid view
                        dataGridView2.Visible = true;
                        PopulateClaimsGrid(token.BodyClaims, dataGridView2);
                        break;
                    default:
                        grid.Rows.Add(key, dictionary[key]);
                        break;
                }

            }
        }
    }
}
