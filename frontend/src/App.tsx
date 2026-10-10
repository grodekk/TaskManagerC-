import { useState, type SubmitEvent } from 'react'
import { login } from './api/authApi'
import { getVehicles } from './api/vehicleApi'
import type { Vehicle } from './types/vehicle'
import { getServiceRecords } from './api/serviceRecordApi'
import type { ServiceRecord } from './types/ServiceRecord'
import LoginForm from './components/LoginForm'
import VehicleList from './components/VehicleList'
import ServiceHistory from './components/ServiceHistory'
import { register } from './api/authApi'
import RegisterForm from './components/RegisterForm'

function App() {
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [vehicles, setVehicles] = useState<Vehicle[]>([])
  const [serviceRecords, setServiceRecords] = useState<ServiceRecord[]>([])
  const [selectedVehicleId, setSelectedVehicleId] = useState<number | null>(null)
  const [isLoggedIn, setIsLoggedIn] = useState(false)
  const [registerUsername, setRegisterUsername] = useState('')
  const [registerPassword, setRegisterPassword] = useState('')
  const [registerMessage, setRegisterMessage] = useState('')
  const [vehicleMessage, setVehicleMessage] = useState('')
  const [loginMessage, setLoginMessage] = useState('')



async function handleSubmit(event: SubmitEvent<HTMLFormElement>) {
  event.preventDefault()

  try {
    const loginData = await login(username, password)

    setLoginMessage('')

    localStorage.setItem('token', loginData.token)
    setIsLoggedIn(true)

    try {
      const vehicleData = await getVehicles(loginData.token)

      setVehicles(vehicleData)
      setVehicleMessage('')
    } catch (error) {
      if (error instanceof Error) {
        setVehicleMessage(error.message)
      }
    }
  } catch (error) {
    console.log(error)
    setLoginMessage('Invalid username or password.')
  }
}

  async function handleRegister(event: SubmitEvent<HTMLFormElement>) {
  event.preventDefault()

  try {
    await register(registerUsername, registerPassword)

    console.log('Registered')

    setRegisterUsername('')
    setRegisterPassword('')
    setRegisterMessage('Account created successfully. You can now log in.')
  } catch (error) {
        if (error instanceof Error) {
          setRegisterMessage(error.message)
        }
    console.log(error)
  }
}

  async function handleVehicleClick(vehicleId: number) {
    const token = localStorage.getItem('token')

    if (!token) {
      return
    }

    try {
      const records = await getServiceRecords(vehicleId, token)

      setSelectedVehicleId(vehicleId)
      setServiceRecords(records)
    } catch (error) {
      console.log(error)
    }
  }


  return (
    <div>
      <h1>Fleet Management</h1>

      {!isLoggedIn && (
        <>
        <LoginForm
          username={username}
          password={password}
          setUsername={setUsername}
          setPassword={setPassword}
          onSubmit={handleSubmit}
        />

        {loginMessage && <p>{loginMessage}</p>}

        <RegisterForm
            username={registerUsername}
            password={registerPassword}
            setUsername={setRegisterUsername}
            setPassword={setRegisterPassword}
            onSubmit={handleRegister}
          />

        {registerMessage && <p>{registerMessage}</p>}
        </>
      )}

      {isLoggedIn && (
        <>
          {vehicleMessage ? (
            <p>{vehicleMessage}</p>
          ) : (
            <>
              <VehicleList
                vehicles={vehicles}
                onVehicleClick={handleVehicleClick}
              />

              <ServiceHistory
                records={serviceRecords}
                visible={selectedVehicleId !== null}
              />
            </>
          )}
        </>
      )}


    </div>
  )
}

export default App