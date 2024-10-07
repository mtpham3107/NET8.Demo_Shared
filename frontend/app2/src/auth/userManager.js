import { UserManager, WebStorageStateStore } from 'oidc-client'

const userManagerConfig = {
  authority: 'https://localhost:7155',
  client_id: 'client_app2',
  client_secret: 'cYakBLjj9o3w9cI3GLpR4ryBHCRcEX3g',
  redirect_uri: `${window.location.origin}/login-callback`,
  response_type: 'code',
  scope: 'openid profile TemplateServiceClientFullAccess GlobalAdminFullAccess',
  post_logout_redirect_uri: `${window.location.origin}/logout-callback`,
  userStore: new WebStorageStateStore({ store: window.localStorage }),
  automaticSilentRenew: true,
  accessTokenExpiringNotificationTime: 60,
  filterProtocolClaims: true,
}

const userManager = new UserManager(userManagerConfig)

export default userManager
