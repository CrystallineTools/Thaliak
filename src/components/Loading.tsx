import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCircleNotch } from '@fortawesome/free-solid-svg-icons';

export default function Loading() {
  return (
    <FontAwesomeIcon icon={faCircleNotch} className='animate-spin text-[5rem] text-gray-600 m-8 w-full' />
  )
}