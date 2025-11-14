import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faPlus, faTrash, faEdit, faSave, faTimes } from '@fortawesome/free-solid-svg-icons';
import {
  listWebhooks,
  createWebhook,
  updateWebhook,
  deleteWebhook,
  type Webhook,
  type CreateWebhookRequest,
} from '../api/authClient';
import Loading from '../components/Loading';
import ListGroup from '../components/list/ListGroup';
import { useAuth } from '../contexts/AuthContext';

export default function WebhooksPage() {
  const navigate = useNavigate();
  const { isAuthenticated, loading: authLoading } = useAuth();
  const [webhooks, setWebhooks] = useState<Webhook[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [editingId, setEditingId] = useState<number | null>(null);
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
          <h1 className='text-3xl font-bold text-gray-800'>Manage Webhooks</h1>
          <button
            onClick={() => setShowCreateForm(!showCreateForm)}
            className='inline-flex items-center gap-2 px-4 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition-colors'>
            <FontAwesomeIcon icon={faPlus} />
            <span>New Webhook</span>
          </button>
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
            selected game services. The payload will include patch details in JSON format.
          </p>
          <details className='text-sm'>
            <summary className='font-medium text-blue-900 cursor-pointer hover:text-blue-700'>Example Payload</summary>
            <pre className='mt-2 p-3 bg-white rounded border border-blue-200 overflow-x-auto text-xs'>
{`{
  "patches": [
    {
      "version_string": "2025.11.13.0000.0001",
      "remote_url": "https://patch-dl.ffxiv.com/game/4e9a232b/D2025.11.13.0000.0001.patch",
      "first_seen": "2025-11-13T12:34:56Z",
      "last_seen": "2025-11-13T12:34:56Z",
      "size": 123456789,
      "hash": {
        "type": "sha1",
        "block_size": 1048576,
        "hashes": ["abc123...", "def456..."]
      },
      "first_offered": "2025-11-13T12:34:56Z",
      "last_offered": "2025-11-13T12:34:56Z",
      "is_active": true
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
