import './global.css';

export const metadata = {
  title: 'IdleGame-DevPanel',
  description: 'IdleGame-DevPanel',
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
