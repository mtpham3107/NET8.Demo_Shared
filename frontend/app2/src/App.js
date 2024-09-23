import React from 'react'
import { BrowserRouter, Route, Routes, Navigate } from 'react-router-dom'
import { AuthProvider, useAuth } from './auth/AuthContext'

import LoginCallback from './auth/LoginCallback'
import LogoutCallback from './auth/LogoutCallback'
import Home from './pages/home/Home'

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path='/login-callback' element={<LoginCallback />} />
          <Route path='/logout-callback' element={<LogoutCallback />} />
          <Route path='/' element={<Home />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  )
}

export default App