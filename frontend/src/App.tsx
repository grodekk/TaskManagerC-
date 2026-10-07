import { useState, type SubmitEvent } from 'react'
import { login } from './api/authApi'
import { getVehicles } from './api/vehicleApi'
import type { Vehicle } from './types/vehicle'

function App() {
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [vehicles, setVehicles] = useState<Vehicle[]>([])

  async function handleSubmit(event: SubmitEvent<HTMLFormElement>) {
    event.preventDefault()

    try {
      const loginData = await login(username, password)

      localStorage.setItem('token', loginData.token)

      const vehicleData = await getVehicles(loginData.token)

      setVehicles(vehicleData)
    } catch (error) {
      console.log(error)
    }
  }

  return (
    <div>
      <h1>Fleet Management</h1>

      <form onSubmit={handleSubmit}>
        <div>
          <label>Username</label>
          <input
            type="text"
            value={username}
            onChange={(event) => setUsername(event.target.value)}
          />
        </div>

        <div>
          <label>Password</label>
          <input
            type="password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
          />
        </div>

        <button type="submit">Login</button>
      </form>

      <h2>Vehicles</h2>

      {vehicles.map((vehicle) => (
        <div key={vehicle.id}>
          {vehicle.registrationNumber}
        </div>
      ))}
    </div>
  )
}

export default App