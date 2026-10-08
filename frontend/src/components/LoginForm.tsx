import type { SubmitEvent } from 'react'

type LoginFormProps = {
  username: string
  password: string
  setUsername: (value: string) => void
  setPassword: (value: string) => void
  onSubmit: (event: SubmitEvent<HTMLFormElement>) => void | Promise<void>
}

function LoginForm({
  username,
  password,
  setUsername,
  setPassword,
  onSubmit
}: LoginFormProps) {
  return (
    <form onSubmit={onSubmit}>
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
  )
}

export default LoginForm