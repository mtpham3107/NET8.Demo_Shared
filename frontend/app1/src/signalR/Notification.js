import React, { useEffect, useState } from 'react'
import { useSignalR } from './SignalRContext'
import { useAuth } from '../auth/AuthContext'

const Notification = () => {
  const { user } = useAuth()
  const { notificationConnection } = useSignalR()
  const [notifications, setNotifications] = useState([])
  const [message, setMessage] = useState('')

  useEffect(() => {
    if (notificationConnection) {
      const handleReceiveNotification = (notification) => {
        console.log(notification)
        // setNotifications((notifications) => [...notifications, notification])
      }

      notificationConnection.on('ReceiveNotification', handleReceiveNotification)

      return () => {
        notificationConnection.off('ReceiveNotification', handleReceiveNotification)
      }
    }
  }, [notificationConnection])

  const sendNotification = async () => {
    if (notificationConnection) {
      try {
        await fetch('https://localhost:7155/api/notifications/send', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${user?.access_token}`,
          },
          body: JSON.stringify(message),
        })
        setMessage('')
      } catch (err) {
        console.error('Error sending notification:', err)
      }
    }
  }

  return (
    <div>
      <h1>Notifications</h1>
      <ul>
        {notifications.map((notification, index) => (
          <li key={index}>{notification}</li>
        ))}
      </ul>
      <input type='text' value={message} onChange={(e) => setMessage(e.target.value)} placeholder='Enter message' />
      <button onClick={sendNotification}>Send Notification to All</button>
    </div>
  )
}

export default Notification
