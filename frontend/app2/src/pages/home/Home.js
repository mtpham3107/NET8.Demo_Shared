import { useAuth } from '../../auth/AuthContext'
import { useState } from 'react'
// import Notification from '../../signalR/Notification'

const Home = () => {
  const { user, login, logout } = useAuth()
  const [result, setResult] = useState()

  const fetchData = async () => {
    try {
      const requestOptions = {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${user?.access_token}`
        }
      }

      fetch('https://localhost:7155/api/users', requestOptions)
        .then((response) => response.text())
        .then((result) => setResult(result))
        .catch((error) => console.error(error))
    } catch (error) {
      console.error(error)
    }
  }

  return (
    <div>
      <button onClick={() => fetchData()}>Test jwt</button>
      {user ? (
        <div>
          <p>Hello, {user.profile.name}</p>
          <p>Bearer {user?.access_token}</p>
          <p>{result}</p>
          <button onClick={() => logout()}>Logout</button>
        </div>
      ) : (
        <button onClick={() => login()}>Login</button>
      )}
    </div>
  )
}

export default Home
