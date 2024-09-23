import { useEffect } from 'react'
import { useAuth } from './AuthContext'

const LogoutCallback = () => {
  const { handleLogoutCallback } = useAuth()

  useEffect(() => {
    handleLogoutCallback()
  }, [])

  return <div>Logout...</div>
}

export default LogoutCallback
