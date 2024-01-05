using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

namespace mvp.tickets.domain.Helpers
{
    public class FirebaseHelper
    {
        public static FirebaseAuth GetFirebaseAuth(string firebaseAdminConfig)
        {
            if (string.IsNullOrWhiteSpace(firebaseAdminConfig)) ThrowHelper.ArgumentNull(nameof(firebaseAdminConfig));

            string instanceName = "Firebase";
            var firebaseApp = FirebaseApp.GetInstance(instanceName);
            if (firebaseApp == null)
            {
                try
                {
                    firebaseApp = FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromJson(firebaseAdminConfig)
                    }, instanceName);
                }
                catch
                {
                    // if Instance was created in another thread
                    firebaseApp = FirebaseApp.GetInstance(instanceName);
                }
            }
            return FirebaseAuth.GetAuth(firebaseApp);
        }
    }
}
