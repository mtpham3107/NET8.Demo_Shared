import { useEffect } from 'react'
import { useAuth } from './AuthContext'

const LoginCallback = () => {
  const { handleLoginCallback } = useAuth()

  useEffect(() => {
    handleLoginCallback()
  }, [])

  return <div>Login...</div>
}

export default LoginCallback
