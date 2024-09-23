import { useEffect } from 'react'
import { useAuth } from '../../auth/AuthContext'

const Home = () => {
  const { user, login, logout } = useAuth()

  const fetchData = async () => {
    try {
      console.log(user?.access_token)

      const myHeaders = new Headers()
      myHeaders.append('Authorization', `Bearer ${user?.access_token}`)

      const requestOptions = {
        method: 'GET',
        headers: myHeaders,
        redirect: 'follow',
      }

      fetch('https://localhost:7086/WeatherForecast', requestOptions)
        .then((response) => response.text())
        .then((result) => console.log(result))
        .catch((error) => console.error(error))
    } catch (error) {
      console.error(error)
    }
  }

  return (
    <div>
      <button onClick={() => fetchData()}>Test data</button>
      {user ? (
        <div>
          <p>Hello, {user.profile.name}</p>
          <button onClick={() => logout()}>Logout</button>
        </div>
      ) : (
        <button onClick={() => login()}>Login</button>
      )}
    </div>
  )
}

export default Home
