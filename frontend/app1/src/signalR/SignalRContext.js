import { createContext, useContext, useEffect, useState, useRef } from 'react'
import { HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr'

const SignalRContext = createContext({
  chatConnection: null,
  notificationConnection: null,
})

export const SignalRProvider = ({ children }) => {
  const [chatConnection, setChatConnection] = useState(null)
  const [notificationConnection, setNotificationConnection] = useState(null)
  // Use useRef to persist connections
  const chatConnectionRef = useRef(null)
  const notificationConnectionRef = useRef(null)

  useEffect(() => {
    if (!chatConnectionRef.current) {
      chatConnectionRef.current = new HubConnectionBuilder().withUrl('https://localhost:7155/chathub').withAutomaticReconnect().build()
    }

    if (!notificationConnectionRef.current) {
      notificationConnectionRef.current = new HubConnectionBuilder().withUrl('https://localhost:7155/notificationHub').withAutomaticReconnect().build()
    }

    const chatHubConnection = chatConnectionRef.current
    const notificationHubConnection = notificationConnectionRef.current

    // startConnection(chatHubConnection, setChatConnection, 'ChatHub');
    startConnection(notificationHubConnection, setNotificationConnection, 'NotificationHub')

    return () => {
      // stopConnection(chatHubConnection, 'ChatHub');
      stopConnection(notificationHubConnection, 'NotificationHub')
    }
  }, [])

  const startConnection = async (connection, setConnection, label) => {
    if (connection.state === HubConnectionState.Disconnected) {
      try {
        await connection.start()
        console.log(`${label} connection successful`)
        setConnection(connection)
      } catch (error) {
        console.error(`${label} connection failed: `, error)
      }
    }
  }

  const stopConnection = async (connection, label) => {
    if (connection.state === HubConnectionState.Connected) {
      try {
        await connection.stop()
        console.log(`${label} disconnected`)
      } catch (error) {
        console.error(`Failed to stop ${label} connection: `, error)
      }
    }
  }

  return <SignalRContext.Provider value={{ chatConnection, notificationConnection }}>{children}</SignalRContext.Provider>
}

export const useSignalR = () => {
  return useContext(SignalRContext)
}
