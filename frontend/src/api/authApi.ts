type LoginResponse = {
  token: string
}

const apiUrl = import.meta.env.VITE_API_URL

export async function login(
  username: string,
  password: string
): Promise<LoginResponse> {
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

    throw new Error(error)
  }

  const data: LoginResponse = await response.json()

  return data
}