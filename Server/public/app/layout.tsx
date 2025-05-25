import './global.css';

export const metadata = {
  title: 'Idle-Game-DevPanel',
  description: 'Idle-Game-DevPanel',
}

export default function RootLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return (
    <html>
      <body>{children}</body>
    </html>
  )
}
