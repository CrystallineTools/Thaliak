export default function SideNavigation({ children }: { children: React.ReactNode }) {
  return (
    <nav className='w-64 h-full overflow-y-auto p-2 bg-gray-100 rounded'>
      {children}
    </nav>
  );
}
