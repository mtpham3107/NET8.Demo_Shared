import React, { createContext, useContext, useEffect, useState } from 'react'
import userManager from './userManager'

const AuthContext = createContext()

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null)
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    const loadUserFromStorage = async () => {
      var url = window.location.href
      const currentUser = await getUser()
      if (currentUser) {
        setUser(currentUser)
      } else {
        login()
      }
      setIsLoading(false)
    }

    if (window.location.pathname !== '/login-callback') {
      loadUserFromStorage()
    }
  }, [])

  const getUser = async () => {
    try {
      var currentUser = await userManager.getUser()
      return currentUser
    } catch (error) {
      console.error('Get User Error:', error)
      return null
    }
  }

  const login = async () => {
    try {
      localStorage.setItem('returnUrl', window.location.href)
      await userManager.signinRedirect()
    } catch (error) {
      console.error('Signin Redirect Error:', error)
    }
  }

  const handleLoginCallback = async () => {
    try {
      var currentUser = await userManager.signinRedirectCallback()

      if (currentUser) {
        await userManager.storeUser(currentUser)
        const returnUrl = localStorage.getItem('returnUrl')
        if (returnUrl) {
          localStorage.removeItem('returnUrl')
          window.location.href = returnUrl
        } else {
          window.location.href = '/'
        }
      }
    } catch (error) {
      console.error('Signin Redirect Callback Error:', error)
    }
  }

  const logout = async () => {
    try {
      await userManager.signoutRedirect()
    } catch (error) {
      console.error('Signout Redirect Error:', error)
    }
  }

  const handleLogoutCallback = async () => {
    try {
      await userManager.signoutRedirectCallback()
      window.location.href = '/'
    } catch (error) {
      console.error('Signout Redirect Callback Error:', error)
    }
  }

  return (
    <AuthContext.Provider
      value={{
        user,
        isLoading,
        login,
        logout,
        handleLoginCallback,
        handleLogoutCallback,
      }}
    >
      {children}
    </AuthContext.Provider>
  )
}

export const useAuth = () => useContext(AuthContext)
