import { UserManager, WebStorageStateStore } from 'oidc-client'

const userManagerConfig = {
  authority: 'https://localhost:7155',
  client_id: 'client_app1',
  client_secret: 'MZ8KUfgCkAgaVLWSkIWDnaxy4TVshc1q',
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
