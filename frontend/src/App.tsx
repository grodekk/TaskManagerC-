import { useState, type SubmitEvent } from 'react'
import { login } from './api/authApi'
import { getVehicles } from './api/vehicleApi'
import type { Vehicle } from './types/vehicle'
import { getServiceRecords } from './api/serviceRecordApi'
import type { ServiceRecord } from './types/ServiceRecord'
import LoginForm from './components/LoginForm'
import VehicleList from './components/VehicleList'
import ServiceHistory from './components/ServiceHistory'

function App() {
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [vehicles, setVehicles] = useState<Vehicle[]>([])
  const [serviceRecords, setServiceRecords] = useState<ServiceRecord[]>([])
  const [selectedVehicleId, setSelectedVehicleId] = useState<number | null>(null)
  const [isLoggedIn, setIsLoggedIn] = useState(false)


  async function handleSubmit(event: SubmitEvent<HTMLFormElement>) {
    event.preventDefault()

    try {
      const loginData = await login(username, password)

      localStorage.setItem('token', loginData.token)
      setIsLoggedIn(true)

      const vehicleData = await getVehicles(loginData.token)

      setVehicles(vehicleData)
    } catch (error) {
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
        <LoginForm
          username={username}
          password={password}
          setUsername={setUsername}
          setPassword={setPassword}
          onSubmit={handleSubmit}
        />
      )}

      {isLoggedIn && (
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

    </div>
  )
}

export default App