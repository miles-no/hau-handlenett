import { PublicClientApplication } from "@azure/msal-browser";
export default defineNuxtPlugin(async () => {
  const config = {
    auth: {
      clientId: import.meta.env.VITE_CLIENT_ID,
      authority: import.meta.env.VITE_AUTHORITY,
      redirectUri: import.meta.env.VITE_REDIRECT_URI,
      knownAuthorities: [import.meta.env.VITE_AUTHORITY],
    },
  };
  const msal = new PublicClientApplication(config);
  try {
    await msal.initialize();
  } catch (e) {
    console.log("MSAL error", e);
  }

  const login = async () => {
    let loginRequest = {
      scopes: [import.meta.env.VITE_SCOPE],
    };
    try {
      let loginResponse = await msal.loginPopup(loginRequest);
      return loginResponse;
    } catch (err) {
      console.log("Login error", err);
      alert("Login error", err);
    }
  };

  let tokenResponse;
  const acquireTokenSilent = async () => {
    const account = msal.getAllAccounts();
    if (account.length > 0) {
      let tokenRequest = {
        scopes: [import.meta.env.VITE_SCOPE],
        account: account[0],
      };
      try {
        tokenResponse = await msal.acquireTokenSilent(tokenRequest);
        return tokenResponse;
      } catch (err) {
        console.log("Token error", err);
      }
    } else {
      return null;
    }
  };
  const getAccounts = () => {
    return msal.getAllAccounts();
  };
  const profileInfo = async () => {
    let payload = await fetch("https://graph.microsoft.com/v1.0/me", {
      headers: {
        Authorization: `Bearer ${tokenResponse.accessToken}`,
      },
    });
    let json = await payload.json();
    return json;
  };

  const logout = async () => {
    const token = await acquireTokenSilent();
    const homeAccountId = token.account.homeAccountId;
    const currentAccount = msal.getAccount(homeAccountId);
    await msal.logoutRedirect({ account: currentAccount });
  };

  return {
    provide: {
      login,
      acquireTokenSilent,
      getAccounts,
      profileInfo,
      logout,
    },
  };
});
