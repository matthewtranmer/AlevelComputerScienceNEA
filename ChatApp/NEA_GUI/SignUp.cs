using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
using System.Text.Json;
using Matthew_Tranmer_NEA.Generic;
using Matthew_Tranmer_NEA.E2E;

namespace NEA_GUI
{
    public partial class SignUp : Form
    {
        public SignUp()
        {
            InitializeComponent();
        }

        private void endOfFunction()
        {
            loading_icon.Visible = false;
            Submit.Enabled = true;
        }

        private (Dictionary<string, string>? response_body, bool fatal_error) uploadInitialKeys()
        {
            //Create two random key pairs.
            KeyPair identity_key = new KeyPair(PreDefinedCurves.nist256);
            KeyPair signed_pre_key = new KeyPair(PreDefinedCurves.nist256);

            //Encode the private keys using base 64.
            string encoded_private_identity_key = Convert.ToBase64String(identity_key.getPrivateComponent().ToByteArray());
            string encoded_private_signed_pre_key = Convert.ToBase64String(signed_pre_key.getPrivateComponent().ToByteArray());

            //Encrypt the private keys so the server can store them without reading them.
            string encrypted_indentity_key = Convert.ToBase64String(Encryption.encrypt(encoded_private_identity_key, ApplicationValues.encryption_key));
            string encrypted_signed_pre_key = Convert.ToBase64String(Encryption.encrypt(encoded_private_signed_pre_key, ApplicationValues.encryption_key));

            //Store the plain text values for later use by the application.
            ApplicationValues.identity_key = identity_key.getPrivateComponent();
            ApplicationValues.signed_pre_key = signed_pre_key.getPrivateComponent();

            //Produce a signature using the two keys so the server can't change the values.
            byte[] signature = EllipticCurveCryptography.generateDSAsignature(identity_key, signed_pre_key.getPublicComponent().getCoordinate().x.ToString()).ToArray();
            string encoded_signature = Convert.ToBase64String(signature);

            Dictionary<string, string> request = new Dictionary<string, string>()
            {
                { "URL", "\\api\\secrets\\upload_initial_keys" },
                { "username", ApplicationValues.username },
                { "token", ApplicationValues.session_token },
                { "private_identity_key", encrypted_indentity_key },
                { "public_identity_key", Convert.ToBase64String(identity_key.getPublicComponent().compressPoint()) },
                { "private_signed_pre_key", encrypted_signed_pre_key },
                { "public_signed_pre_key", Convert.ToBase64String(signed_pre_key.getPublicComponent().compressPoint()) },
                { "pre_key_signature", encoded_signature }
            };

            //Send request to server.
            return API.apiRequest(request);
        }

        private (Dictionary<string, string>? response_body, bool fatal_error) uploadPreKeys()
        {
            //Generate pre keys.
            const int number_of_pre_keys = 5;
            for (int i = 0; i < number_of_pre_keys; i++)
            {
                KeyPair key_pair = new KeyPair(PreDefinedCurves.nist256);
                PreKey pre_key = new PreKey(key_pair);

                string encoded_private_key = Convert.ToBase64String(key_pair.getPrivateComponent().ToByteArray());
                string encrypted_private_key = Convert.ToBase64String(Encryption.encrypt(encoded_private_key, ApplicationValues.encryption_key));

                ApplicationValues.pre_keys.Add(new PreKey(key_pair));

                Dictionary<string, string> pre_key_request = new Dictionary<string, string>()
                {
                    { "URL", "\\api\\secrets\\upload_pre_key" },
                    { "username", ApplicationValues.username },
                    { "token", ApplicationValues.session_token },
                    { "identifier", pre_key.identifier },
                    { "encrypted_private_key", encrypted_private_key },
                    { "encoded_public_key", Convert.ToBase64String(key_pair.getPublicComponent().compressPoint()) }
                };

                //Send upload pre key request.
                var response = API.apiRequest(pre_key_request);
                if (response.fatal_error)
                {
                    return response;
                }
            }

            return (null, false);
        }

        //Creates a request to the server to create a new account.
        private async void submitClick(object sender, EventArgs e)
        {
            //Reset error label message.
            Submit.Enabled = false;
            error_label.Text = "";
            loading_icon.Visible = true;

            //If inputted passwords don't match.
            if (Password_Input.Text != retype_password_input.Text)
            {
                error_label.Text = "Passwords Don't Match.";
                endOfFunction();
                return;
            }

            string username = Username_Input.Text;
            //Derive two constants from the password.
            var constants = Functions.deriveConstants(Password_Input.Text);

            //Make a create account request to the server.
            Dictionary<string, string> request = new Dictionary<string, string>()
            {
                { "URL", "\\api\\management\\create_account" },
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

                //Generate and upload the initial keys. 
                (response, fatal_error) = await Task.Run(uploadInitialKeys);
                if (fatal_error)
                {
                    endOfFunction();
                    return;
                }

                //Generate and upload the pre keys.
                (response, fatal_error) = await Task.Run(uploadPreKeys);
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

        //If user accidentially pressed signup.
        private void loginLabelClick(object sender, EventArgs e)
        {
            Hide();
            Login form = new Login();
            form.ShowDialog();
            Close();
        }
    }
}
