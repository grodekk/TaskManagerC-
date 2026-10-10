import type { SubmitEvent } from 'react'

type RegisterFormProps = {
  username: string
  password: string
  setUsername: (value: string) => void
  setPassword: (value: string) => void
  onSubmit: (event: SubmitEvent<HTMLFormElement>) => void | Promise<void>
}

function RegisterForm({
  username,
  password,
  setUsername,
  setPassword,
  onSubmit
}: RegisterFormProps) {
  return (
    <form onSubmit={onSubmit}>
      <h2>Register</h2>

      <div>
        <label>Username</label>
        <input
          type="text"
          value={username}
          onChange={(event) => setUsername(event.target.value)}
          required
        />
      </div>

      <div>
        <label>Password</label>
        <input
          type="password"
          value={password}
          onChange={(event) => setPassword(event.target.value)}
          required
        />
      </div>

      <button type="submit">Register</button>
    </form>
  )
}

export default RegisterForm