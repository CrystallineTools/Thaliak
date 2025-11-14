import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faPlus, faTrash, faEdit, faSave, faTimes, faPaperPlane } from '@fortawesome/free-solid-svg-icons';
import {
  listWebhooks,
  createWebhook,
  updateWebhook,
  deleteWebhook,
  testWebhook,
  type Webhook,
  type CreateWebhookRequest,
} from '../api/authClient';
import Loading from '../components/Loading';
import ListGroup from '../components/list/ListGroup';
import { useAuth } from '../contexts/AuthContext';
import { discordLink } from '../constants';

export default function WebhooksPage() {
  const navigate = useNavigate();
  const { isAuthenticated, loading: authLoading } = useAuth();
  const [webhooks, setWebhooks] = useState<Webhook[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [testingId, setTestingId] = useState<number | null>(null);
  const [formData, setFormData] = useState<CreateWebhookRequest>({
    url: '',
    subscribe_jp: true,
    subscribe_kr: false,
    subscribe_cn: false,
  });

  useEffect(() => {
    if (authLoading) {
      return;
    }

    if (!isAuthenticated) {
      navigate('/');
      return;
    }

    loadWebhooks();
  }, [isAuthenticated, authLoading, navigate]);

  const loadWebhooks = async () => {
    try {
      setLoading(true);
      const response = await listWebhooks();
      setWebhooks(response.webhooks);
      setError(null);
    } catch (err) {
      console.error('Failed to load webhooks:', err);
      setError('Failed to load webhooks. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async () => {
    if (!formData.url) {
      alert('Please enter a webhook URL');
      return;
    }

    try {
      await createWebhook(formData);
      setShowCreateForm(false);
      setFormData({
        url: '',
        subscribe_jp: true,
        subscribe_kr: false,
        subscribe_cn: false,
      });
      await loadWebhooks();
    } catch (err) {
      console.error('Failed to create webhook:', err);
      alert('Failed to create webhook. Please check the URL and try again.');
    }
  };

  const handleUpdate = async (id: number) => {
    try {
      await updateWebhook(id, formData);
      setEditingId(null);
      setFormData({
        url: '',
        subscribe_jp: true,
        subscribe_kr: false,
        subscribe_cn: false,
      });
      await loadWebhooks();
    } catch (err) {
      console.error('Failed to update webhook:', err);
      alert('Failed to update webhook. Please try again.');
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm('Are you sure you want to delete this webhook?')) {
      return;
    }

    try {
      await deleteWebhook(id);
      await loadWebhooks();
    } catch (err) {
      console.error('Failed to delete webhook:', err);
      alert('Failed to delete webhook. Please try again.');
    }
  };

  const handleTest = async (id: number) => {
    if (!confirm('Send a test webhook with the latest 3 patches from each subscribed repository?')) {
      return;
    }

    try {
      setTestingId(id);
      const response = await testWebhook(id);
      alert(`Test webhook sent successfully! Sent ${response.new_patches?.length || 0} repository patch groups.`);
    } catch (err) {
      console.error('Failed to test webhook:', err);
      alert('Failed to test webhook. Please check the webhook URL and try again.');
    } finally {
      setTestingId(null);
    }
  };

  const startEdit = (webhook: Webhook) => {
    setEditingId(webhook.id);
    setFormData({
      url: webhook.url,
      subscribe_jp: webhook.subscribe_jp,
      subscribe_kr: webhook.subscribe_kr,
      subscribe_cn: webhook.subscribe_cn,
    });
  };

  const cancelEdit = () => {
    setEditingId(null);
    setFormData({
      url: '',
      subscribe_jp: true,
      subscribe_kr: false,
      subscribe_cn: false,
    });
  };

  if (authLoading || loading) {
    return (
      <div className='container mx-auto px-4 mt-8'>
        <Loading />
      </div>
    );
  }

  return (
    <div className='container mx-auto px-4 mt-8 pb-8'>
      <div className='max-w-4xl mx-auto'>
        <div className='flex items-center justify-between mb-6'>
          <h1 className='text-3xl font-bold text-gray-800 inline-flex items-center gap-3'>
            Manage Webhooks
            <span className='inline-flex items-center px-2 py-1 text-xs font-bold uppercase bg-amber-400 text-amber-900 rounded'>
              Beta
            </span>
          </h1>
          <button
            onClick={() => setShowCreateForm(!showCreateForm)}
            className='inline-flex items-center gap-2 px-4 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition-colors'>
            <FontAwesomeIcon icon={faPlus} />
            <span>New Webhook</span>
          </button>
        </div>

        <div className='mb-6 p-4 bg-amber-50 border border-amber-200 rounded-lg'>
          <h3 className='text-sm font-bold text-amber-900 mb-2'>Beta Feature - No Stability Guarantees</h3>
          <p className='text-sm text-amber-800 mb-2'>
            Webhooks are currently in beta with <strong>no expectation of stability</strong>:
          </p>
          <ul className='text-sm text-amber-800 list-disc list-inside space-y-1 mb-3'>
            <li><strong>Payload structure may change</strong> without notice or versioning</li>
            <li><strong>Delivery is not guaranteed</strong> and may be unreliable</li>
            <li>Webhooks may be disabled, modified, or removed at any time</li>
          </ul>
          <p className='text-sm text-amber-800'>
            Do not rely on webhooks for production or critical systems. Questions?{' '}
            <a
              href={discordLink}
              target='_blank'
              rel='noopener noreferrer'
              className='font-medium text-amber-900 hover:text-amber-950 underline'>
              Ask in Discord
            </a>
          </p>
        </div>

        {error && (
          <div className='mb-4 p-4 bg-red-50 border border-red-200 rounded-lg text-red-700'>
            {error}
          </div>
        )}

        {showCreateForm && (
          <div className='mb-6 bg-white rounded-lg shadow-lg p-6'>
            <h2 className='text-xl font-bold text-gray-800 mb-4'>Create New Webhook</h2>
            <WebhookForm
              formData={formData}
              setFormData={setFormData}
              onSave={handleCreate}
              onCancel={() => {
                setShowCreateForm(false);
                setFormData({
                  url: '',
                  subscribe_jp: true,
                  subscribe_kr: false,
                  subscribe_cn: false,
                });
              }}
            />
          </div>
        )}

        {webhooks.length === 0 ? (
          <ListGroup>
            <div className='text-center py-8 text-gray-500'>
              <p>No webhooks configured yet.</p>
              <p className='text-sm mt-2'>Click "New Webhook" to create one.</p>
            </div>
          </ListGroup>
        ) : (
          <div className='space-y-4'>
            {webhooks.map((webhook) => (
              <ListGroup key={webhook.id}>
                {editingId === webhook.id ? (
                  <div className='p-4'>
                    <WebhookForm
                      formData={formData}
                      setFormData={setFormData}
                      onSave={() => handleUpdate(webhook.id)}
                      onCancel={cancelEdit}
                    />
                  </div>
                ) : (
                  <div className='p-4'>
                    <div className='flex items-start justify-between mb-3'>
                      <div className='flex-1'>
                        <div className='font-mono text-sm text-gray-800 break-all mb-2'>
                          {webhook.url}
                        </div>
                        <div className='flex gap-2'>
                          {webhook.subscribe_jp && (
                            <span className='px-2 py-1 text-xs bg-blue-100 text-blue-800 rounded'>JP/Global</span>
                          )}
                          {webhook.subscribe_kr && (
                            <span className='px-2 py-1 text-xs bg-green-100 text-green-800 rounded'>KR</span>
                          )}
                          {webhook.subscribe_cn && (
                            <span className='px-2 py-1 text-xs bg-red-100 text-red-800 rounded'>CN</span>
                          )}
                        </div>
                        <div className='text-xs text-gray-500 mt-2'>
                          Created: {new Date(webhook.created_at).toLocaleString()}
                        </div>
                      </div>
                      <div className='flex gap-2 ml-4'>
                        <button
                          onClick={() => handleTest(webhook.id)}
                          disabled={testingId === webhook.id}
                          className='inline-flex items-center gap-1.5 px-3 py-1.5 text-sm text-green-600 hover:bg-green-50 rounded transition-colors disabled:opacity-50 disabled:cursor-not-allowed border border-green-300'
                          title='Send test webhook'>
                          <FontAwesomeIcon icon={faPaperPlane} spin={testingId === webhook.id} />
                          <span>Test</span>
                        </button>
                        <button
                          onClick={() => startEdit(webhook)}
                          className='p-2 text-blue-600 hover:bg-blue-50 rounded transition-colors'>
                          <FontAwesomeIcon icon={faEdit} />
                        </button>
                        <button
                          onClick={() => handleDelete(webhook.id)}
                          className='p-2 text-red-600 hover:bg-red-50 rounded transition-colors'>
                          <FontAwesomeIcon icon={faTrash} />
                        </button>
                      </div>
                    </div>
                  </div>
                )}
              </ListGroup>
            ))}
          </div>
        )}

        <div className='mt-8 p-4 bg-blue-50 border border-blue-200 rounded-lg'>
          <h3 className='font-bold text-blue-900 mb-2'>About Webhooks</h3>
          <p className='text-sm text-blue-800 mb-3'>
            Webhooks will receive POST requests with new patch information whenever new patches are detected for the
            selected game services. The payload includes patches grouped by repository in JSON format.
          </p>
          <p className='text-sm text-blue-800 mb-3'>
            <strong>Note:</strong> The payload structure shown below is current as of now, but may change at any time without notice during the beta period.
          </p>
          <p className='text-sm text-blue-800 mb-3'>
            Use the <FontAwesomeIcon icon={faPaperPlane} className='text-green-600' /> button next to each webhook to
            send a test payload with the latest 3 patches from each repository.
          </p>
          <details className='text-sm'>
            <summary className='font-medium text-blue-900 cursor-pointer hover:text-blue-700'>Example Payload</summary>
            <pre className='mt-2 p-3 bg-white rounded border border-blue-200 overflow-x-auto text-xs'>
{`{
  "new_patches": [
    {
      "repository": {
        "service_id": "jp",
        "slug": "2b5cbc63",
        "name": "ffxivneo/win32/release/boot",
        "description": "FFXIV Global/JP - Retail - Boot - Win32"
      },
      "patches": [
        {
          "version_string": "2025.07.17.0000.0001",
          "remote_url": "http://patch-dl.ffxiv.com/boot/2b5cbc63/D2025.07.17.0000.0001.patch",
          "first_seen": "2025-08-05T02:28:29.309197Z",
          "last_seen": "2025-11-13T06:47:29.717800166Z",
          "size": 20765607,
          "hash": {
            "type": "none"
          },
          "first_offered": "2025-08-05T02:28:29.309197Z",
          "last_offered": "2025-11-13T06:47:29.717800166Z",
          "is_active": true
        },
        {
          "version_string": "2025.05.01.0000.0001",
          "remote_url": "http://patch-dl.ffxiv.com/boot/2b5cbc63/D2025.05.01.0000.0001.patch",
          "first_seen": "2025-05-27T07:25:50.822517Z",
          "last_seen": "2025-08-05T02:27:26.602577Z",
          "size": 20749351,
          "hash": {
            "type": "none"
          },
          "first_offered": "2025-05-27T07:25:50.822517Z",
          "last_offered": "2025-08-05T02:27:26.602577Z",
          "is_active": false
        }
      ]
    },
    {
      "repository": {
        "service_id": "jp",
        "slug": "4e9a232b",
        "name": "ffxivneo/win32/release/game",
        "description": "FFXIV Global/JP - Retail - Base Game - Win32"
      },
      "patches": [
        {
          "version_string": "2025.10.30.0000.0000",
          "remote_url": "http://patch-dl.ffxiv.com/game/4e9a232b/D2025.10.30.0000.0000.patch",
          "first_seen": "2025-11-11T07:01:27.879803Z",
          "last_seen": "2025-11-13T06:47:33.688054283Z",
          "size": 19737190,
          "hash": {
            "type": "sha1",
            "block_size": 50000000,
            "hashes": [
              "a2e46a132ab717bfc27a95fca835017e0471eb4e"
            ]
          },
          "first_offered": "2025-11-11T07:01:27.879803Z",
          "last_offered": "2025-11-13T06:47:33.688054283Z",
          "is_active": true
        }
      ]
    }
  ]
}`}
            </pre>
          </details>
        </div>
      </div>
    </div>
  );
}

interface WebhookFormProps {
  formData: CreateWebhookRequest;
  setFormData: (data: CreateWebhookRequest) => void;
  onSave: () => void;
  onCancel: () => void;
}

function WebhookForm({ formData, setFormData, onSave, onCancel }: WebhookFormProps) {
  return (
    <div className='space-y-4'>
      <div>
        <label className='block text-sm font-medium text-gray-700 mb-1'>Webhook URL</label>
        <input
          type='url'
          value={formData.url}
          onChange={(e) => setFormData({ ...formData, url: e.target.value })}
          placeholder='https://your-server.com/webhook'
          className='w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500'
        />
      </div>

      <div>
        <label className='block text-sm font-medium text-gray-700 mb-2'>Subscribe to Services</label>
        <div className='space-y-2'>
          <label className='flex items-center gap-2'>
            <input
              type='checkbox'
              checked={formData.subscribe_jp}
              onChange={(e) => setFormData({ ...formData, subscribe_jp: e.target.checked })}
              className='rounded'
            />
            <span className='text-sm'>JP/Global (Japan & NA/EU)</span>
          </label>
          <label className='flex items-center gap-2'>
            <input
              type='checkbox'
              checked={formData.subscribe_kr}
              onChange={(e) => setFormData({ ...formData, subscribe_kr: e.target.checked })}
              className='rounded'
            />
            <span className='text-sm'>KR (Korea)</span>
          </label>
          <label className='flex items-center gap-2'>
            <input
              type='checkbox'
              checked={formData.subscribe_cn}
              onChange={(e) => setFormData({ ...formData, subscribe_cn: e.target.checked })}
              className='rounded'
            />
            <span className='text-sm'>CN (China)</span>
          </label>
        </div>
      </div>

      <div className='flex gap-2'>
        <button
          onClick={onSave}
          className='inline-flex items-center gap-2 px-4 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition-colors'>
          <FontAwesomeIcon icon={faSave} />
          <span>Save</span>
        </button>
        <button
          onClick={onCancel}
          className='inline-flex items-center gap-2 px-4 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300 transition-colors'>
          <FontAwesomeIcon icon={faTimes} />
          <span>Cancel</span>
        </button>
      </div>
    </div>
  );
}
