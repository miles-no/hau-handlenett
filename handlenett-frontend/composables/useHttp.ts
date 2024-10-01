export default async function (url: string, method: string, data = {}) {
  const { $acquireTokenSilent } = useNuxtApp();

  const token = await $acquireTokenSilent();
  const baseURL = import.meta.env.VITE_BASE_URL;

  try {
    const response = await fetch(`${baseURL}${url}`, {
      method: method,
      headers: {
        Authorization: `Bearer ${token?.accessToken}`,
        "Content-type": "application/json",
      },
      mode: "cors",
      body: !!data && method != "GET" ? JSON.stringify(data) : null,
    });
    return response.json();
  } catch (error) {
    console.info(error);
    return Promise.reject(error);
  }
}
