import { useState, type SubmitEvent } from 'react'

function App() {
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')

  async function handleSubmit(event: SubmitEvent<HTMLFormElement>) {
    event.preventDefault()

    const apiUrl = import.meta.env.VITE_API_URL

    const response = await fetch(`${apiUrl}/api/auth/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        username,
        password
      })
    })

  if (!response.ok) {
    const error = await response.text()

    console.log(response.status)
    console.log(error)

    return
  }

  const data = await response.json()

  localStorage.setItem('token', data.token)

  console.log('Logged in')
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
    </div>
  )
}

export default App