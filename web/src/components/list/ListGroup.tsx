export default function ListGroup({ children, ...props }: { children?: React.ReactNode }) {
  return (
    <div className='flex flex-col mb-6 bg-white rounded-lg shadow-soft border border-gray-200 overflow-hidden transition-shadow hover:shadow-soft-lg' {...props}>
      {children}
    </div>
  );
}
