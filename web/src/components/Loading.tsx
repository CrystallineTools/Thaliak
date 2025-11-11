import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCircleNotch } from '@fortawesome/free-solid-svg-icons';

export default function Loading() {
  return (
    <div className='flex items-center justify-center min-h-[400px] w-full'>
      <FontAwesomeIcon icon={faCircleNotch} className='animate-spin text-[4rem] text-primary-500' />
    </div>
  )
}