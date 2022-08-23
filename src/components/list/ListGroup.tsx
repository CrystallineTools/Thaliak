export default function ListGroup({ children, ...props }: { children?: React.ReactNode }) {
  return (
    <div className='flex flex-col px-4 mb-4 border rounded border-gray-300' {...props}>
      {children}
    </div>
  );
}
