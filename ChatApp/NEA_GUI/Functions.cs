using Matthew_Tranmer_NEA.Generic;

namespace NEA_GUI
{
    internal static class Functions
    {
        //Displays an error with details if nessasary.
        public static void showError(string details = "")
        {
            string message = "There has been an error whilst performing your request";
            if (details != "")
            {
                message += "\n\nDetails:\n" + details;
            }

            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        //Derive two values from the password.
        public static (string authentication_code, string encryption_key) deriveConstants(string password)
        {
            //Generate a hash of the password.
            byte[] hashBytes = Hash.generateHashBytes(password);
            byte first_byte = hashBytes[0];

            //Derive the first value.
            hashBytes[0] = (byte)(first_byte ^ 0x47);
            string authentication_code = Hash.generateHash(Convert.ToBase64String(hashBytes));

            //Derive the second value.
            hashBytes[0] = (byte)(first_byte ^ 0xAD);
            string encryption_key = Hash.generateHash(Convert.ToBase64String(hashBytes));

            return (authentication_code, encryption_key);
        }
    }
}
