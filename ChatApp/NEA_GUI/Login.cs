using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Matthew_Tranmer_NEA.Generic;
using Matthew_Tranmer_NEA.E2E;
using System.Numerics;
using System.Text.Json;

namespace NEA_GUI
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void endOfFunction()
        {
            loading_icon.Visible = false;
            Submit.Enabled = true;
        }

        private (Dictionary<string, string>? response_body, bool fatal_error) downloadPrivateKeys()
        {
            Dictionary<string, string> request = new Dictionary<string, string>()
            {
                { "URL", "\\api\\secrets\\download_private_keys" },
                { "username", ApplicationValues.username },
                { "token", ApplicationValues.session_token }
            };

            var tuple_response = API.apiRequest(request);
            //If the server couldn't complete the task, stop execution.
            if (tuple_response.fatal_error)
            {
                return tuple_response;
            }

            Dictionary<string, string>? response = tuple_response.response_body;

            //Decrypt private keys from server and save them for later use.
            byte[] encrypted_private_identity_key = Convert.FromBase64String(response!["private_identity_key"]);
            byte[] encrypted_signed_pre_key = Convert.FromBase64String(response!["private_signed_pre_key"]);

            byte[] encoded_private_identity_key = Convert.FromBase64String(Encryption.decrypt(encrypted_private_identity_key, ApplicationValues.encryption_key));
            byte[] encoded_signed_pre_key = Convert.FromBase64String(Encryption.decrypt(encrypted_signed_pre_key, ApplicationValues.encryption_key));

            ApplicationValues.identity_key = new BigInteger(encoded_private_identity_key);
            ApplicationValues.signed_pre_key = new BigInteger(encoded_signed_pre_key);

            return (null, false);
        }

        private (Dictionary<string, string>? response_body, bool fatal_error) downloadPreKeys()
        {
            Dictionary<string, string> request = new Dictionary<string, string>()
            {
                { "URL", "\\api\\secrets\\download_pre_keys" },
                { "username", ApplicationValues.username },
                { "token", ApplicationValues.session_token }
            };

            var tuple_response = API.apiRequest(request);
            //If the server couldn't complete the task, stop execution.
            if (tuple_response.fatal_error)
            {
                return tuple_response;
            }

            Dictionary<string, string>? response = tuple_response.response_body;
            List<string>? pre_keys = JsonSerializer.Deserialize<List<string>>(response!["pre_keys"]);
            foreach (string encoded_pre_key in pre_keys!)
            {
                Dictionary<string, string>? deserialized_pre_key = JsonSerializer.Deserialize<Dictionary<string, string>>(encoded_pre_key);

                byte[] encrypted_private_value = Convert.FromBase64String(deserialized_pre_key!["private_value"]);
                string encoded_private_value = Encryption.decrypt(encrypted_private_value, ApplicationValues.encryption_key);
                BigInteger private_value = new BigInteger(Convert.FromBase64String(encoded_private_value));

                KeyPair key_pair = new KeyPair(private_value, PreDefinedCurves.nist256);
                ApplicationValues.pre_keys.Add(new PreKey(key_pair, deserialized_pre_key["identifier"]));
            }

            return (null, false);
        }

        //Creates a request to the server to generate a new session token.
        private async void submitClick(object sender, EventArgs e)
        {
            //Reset error label message.
            Submit.Enabled = false;
            error_label.Text = "";
            loading_icon.Visible = true;

            string username = Username_Input.Text;
            //Derive two constants from the password.
            var constants = Functions.deriveConstants(Password_Input.Text);

            //Make a generate token request to the server.
            Dictionary<string, string> request = new Dictionary<string, string>()
            {
                { "URL", "\\api\\management\\generate_token" },
                { "authentication_code", constants.authentication_code },
                { "username", username }
            };

            (Dictionary<string, string>? response, bool fatal_error) = await Task.Run(() => API.apiRequest(request));
            //If the server couldn't complete the task, stop execution.
            if (fatal_error)
            {
                endOfFunction();
                return;
            }

            //If the request was successful.
            if (response.ContainsKey("session_token"))
            {
                //Set values that will be needed in the future.
                ApplicationValues.username = username;
                ApplicationValues.session_token = response["session_token"];
                ApplicationValues.encryption_key = constants.encryption_key;

                (response, fatal_error) = await Task.Run(downloadPrivateKeys);
                if (fatal_error)
                {
                    endOfFunction();
                    return;
                }

                (response, fatal_error) = await Task.Run(downloadPreKeys);
                if (fatal_error)
                {
                    endOfFunction();
                    return;
                }

                //Show the main screen.
                Hide();
                MainForm form = new MainForm();
                form.ShowDialog();
                Close();

                //Stop execution.
                return;
            }

            //If an error was returned.
            if (response.ContainsKey("error"))
            {
                //Set the error label to the error response sent by the server.
                error_label.Text = response["error"];
                endOfFunction();
            }
        }

        //If user doesn't have an account.
        private void signUpLabelClick(object sender, EventArgs e)
        {
            Hide();
            SignUp form = new SignUp();
            form.ShowDialog();
            Close();
        }
    }
}
