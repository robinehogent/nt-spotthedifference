export default function Page({ children }: { children: React.ReactNode }) {
  return (
    <div className="page-container">
      <div className="card">{children}</div>
    </div>
  );
}
