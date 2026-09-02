// Lets the api layer report "the server rejected our session" without importing React or the
// router (which would make AuthContext -> api -> AuthContext circular). AuthProvider registers
// the handler; httpClient/graphqlClient just fire it on a 401.
let unauthorizedHandler: (() => void) | null = null;

export function setUnauthorizedHandler(handler: () => void) {
  unauthorizedHandler = handler;
}

export function notifyUnauthorized() {
  // Only meaningful if we actually had a session — a 401 from the login request itself
  // means wrong credentials, not an expired session.
  if (localStorage.getItem('auth')) {
    unauthorizedHandler?.();
  }
}
