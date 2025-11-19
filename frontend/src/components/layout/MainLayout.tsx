import React, { useState } from 'react';
import {
  Box,
  AppBar,
  Toolbar,
  Drawer,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  Avatar,
  Menu,
  MenuItem,
  Divider,
  Typography,
  IconButton,
  Badge,
  Collapse,
} from '@mui/material';
import {
  Menu as MenuIcon,
  Close as CloseIcon,
  Dashboard as DashboardIcon,
  People as PeopleIcon,
  Settings as SettingsIcon,
  Logout as LogoutIcon,
  Notifications as NotificationsIcon,
  AccountCircle as AccountCircleIcon,
  ExpandLess,
  ExpandMore,
  PersonAdd as PersonAddIcon,
  History as HistoryIcon,
  Security as SecurityIcon,
  Business as BusinessIcon,
  Warehouse as WarehouseIcon,
} from '@mui/icons-material';
import { useNavigate, useLocation, Outlet } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';

// ============================================================================
// Layout Principal
// ============================================================================

const DRAWER_WIDTH = 280;

export const MainLayout: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { user, logout } = useAuth();
  const [mobileOpen, setMobileOpen] = useState(false);
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [openSubmenu, setOpenSubmenu] = useState<string | null>(null);

  const handleDrawerToggle = () => {
    setMobileOpen(!mobileOpen);
  };

  const handleMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
  };

  const handleLogout = () => {
    handleMenuClose();
    logout();
  };

  const handleSubmenuToggle = (menuLabel: string) => {
    setOpenSubmenu(openSubmenu === menuLabel ? null : menuLabel);
  };

  const menuItems = [
    {
      label: 'Dashboard',
      icon: <DashboardIcon sx={{ color: '#2196f3' }} />,
      path: '/dashboard',
      color: '#2196f3',
    },
    {
      label: 'Cadastros',
      icon: <PersonAddIcon sx={{ color: '#4caf50' }} />,
      color: '#4caf50',
      submenu: [
        {
          label: 'Empresa',
          icon: <BusinessIcon sx={{ color: '#00897b' }} />,
          path: '/company',
          color: '#00897b',
        },
        {
          label: 'Armazéns',
          icon: <WarehouseIcon sx={{ color: '#5e35b1' }} />,
          path: '/warehouses',
          color: '#5e35b1',
        },
        {
          label: 'Clientes',
          icon: <PeopleIcon sx={{ color: '#ff5722' }} />,
          path: '/customers',
          color: '#ff5722',
        },
      ],
    },
    {
      label: 'Usuários',
      icon: <PeopleIcon sx={{ color: '#9c27b0' }} />,
      color: '#9c27b0',
      submenu: [
        {
          label: 'Cadastrar',
          icon: <PersonAddIcon sx={{ color: '#4caf50' }} />,
          path: '/users/register',
          color: '#4caf50',
        },
        {
          label: 'Atividades',
          icon: <HistoryIcon sx={{ color: '#ff9800' }} />,
          path: '/users/activities',
          color: '#ff9800',
        },
        {
          label: 'Permissões',
          icon: <SecurityIcon sx={{ color: '#f44336' }} />,
          path: '/users/permissions',
          color: '#f44336',
        },
      ],
    },
    {
      label: 'Configurações',
      icon: <SettingsIcon sx={{ color: '#607d8b' }} />,
      color: '#607d8b',
      submenu: [],
    },
  ];

  const drawerContent = (
    <Box
      sx={{
        display: 'flex',
        flexDirection: 'column',
        height: '100%',
        backgroundColor: '#f5f5f5',
      }}
    >
      {/* Logo */}
      <Box
        sx={{
          p: 2,
          textAlign: 'center',
          borderBottom: '1px solid #e0e0e0',
        }}
      >
        <Typography variant="h6" sx={{ fontWeight: 'bold', color: '#1976d2' }}>
          🚀 WMS
        </Typography>
        <Typography variant="caption" display="block" sx={{ color: 'textSecondary' }}>
          Interprise
        </Typography>
      </Box>

      {/* User Info */}
      <Box
        sx={{
          p: 2,
          display: 'flex',
          alignItems: 'center',
          gap: 2,
          borderBottom: '1px solid #e0e0e0',
        }}
      >
        <Avatar sx={{ width: 40, height: 40, backgroundColor: '#1976d2' }}>
          {user?.firstName?.charAt(0) ?? 'U'}
        </Avatar>
        <Box sx={{ flex: 1, minWidth: 0 }}>
          <Typography variant="body2" sx={{ fontWeight: 'bold' }}>
            {user?.firstName} {user?.lastName}
          </Typography>
          <Typography variant="caption" sx={{ color: 'textSecondary', display: 'block', overflow: 'hidden', textOverflow: 'ellipsis' }}>
            {user?.email}
          </Typography>
        </Box>
      </Box>

      {/* Menu Items */}
      <List sx={{ flex: 1 }}>
        {menuItems.map((item) => (
          <React.Fragment key={item.label}>
            <ListItem
              button
              onClick={() => {
                if (item.submenu) {
                  handleSubmenuToggle(item.label);
                } else if (item.path) {
                  navigate(item.path);
                  setMobileOpen(false);
                }
              }}
              selected={!item.submenu && location.pathname === item.path}
              sx={{
                '&.Mui-selected': {
                  backgroundColor: '#e3f2fd',
                  borderLeft: `4px solid ${item.color || '#1976d2'}`,
                  '& .MuiListItemIcon-root': {
                    color: item.color || '#1976d2',
                  },
                  '& .MuiListItemText-primary': {
                    color: item.color || '#1976d2',
                    fontWeight: 'bold',
                  },
                },
                '&:hover': {
                  backgroundColor: '#f0f0f0',
                },
              }}
            >
              <ListItemIcon>{item.icon}</ListItemIcon>
              <ListItemText primary={item.label} />
              {item.submenu && (
                openSubmenu === item.label ? <ExpandLess /> : <ExpandMore />
              )}
            </ListItem>

            {/* Submenu */}
            {item.submenu && (
              <Collapse in={openSubmenu === item.label} timeout="auto" unmountOnExit>
                <List component="div" disablePadding>
                  {item.submenu.map((subItem) => (
                    <ListItem
                      button
                      key={subItem.path}
                      onClick={() => {
                        navigate(subItem.path);
                        setMobileOpen(false);
                      }}
                      selected={location.pathname === subItem.path}
                      sx={{
                        pl: 8,
                        '&.Mui-selected': {
                          backgroundColor: '#e3f2fd',
                          borderLeft: `4px solid ${subItem.color || '#1976d2'}`,
                          '& .MuiListItemIcon-root': {
                            color: subItem.color || '#1976d2',
                          },
                          '& .MuiListItemText-primary': {
                            color: subItem.color || '#1976d2',
                            fontWeight: 'bold',
                          },
                        },
                        '&:hover': {
                          backgroundColor: '#f0f0f0',
                        },
                      }}
                    >
                      <ListItemIcon sx={{ minWidth: 40 }}>{subItem.icon}</ListItemIcon>
                      <ListItemText primary={subItem.label} />
                    </ListItem>
                  ))}
                </List>
              </Collapse>
            )}
          </React.Fragment>
        ))}
      </List>

      {/* Footer */}
      <Box
        sx={{
          p: 2,
          borderTop: '1px solid #e0e0e0',
          textAlign: 'center',
        }}
      >
        <Typography variant="caption" display="block" sx={{ color: 'textSecondary' }}>
          v1.0.0
        </Typography>
      </Box>
    </Box>
  );

  return (
    <Box sx={{ display: 'flex', minHeight: '100vh' }}>
      {/* App Bar */}
      <AppBar
        position="fixed"
        sx={{
          zIndex: (theme) => theme.zIndex.drawer + 1,
          boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
        }}
      >
        <Toolbar>
          <IconButton
            color="inherit"
            onClick={handleDrawerToggle}
            sx={{ mr: 2, display: { sm: 'none' } }}
          >
            {mobileOpen ? <CloseIcon /> : <MenuIcon />}
          </IconButton>

          <Typography
            variant="h6"
            sx={{
              flexGrow: 1,
              fontWeight: 'bold',
            }}
          >
            WMS Interprise
          </Typography>

          {/* Notifications */}
          <IconButton color="inherit" sx={{ mr: 2 }}>
            <Badge badgeContent={3} color="error">
              <NotificationsIcon />
            </Badge>
          </IconButton>

          {/* User Menu */}
          <IconButton
            color="inherit"
            onClick={handleMenuOpen}
            sx={{
              ml: 1,
              display: 'flex',
              alignItems: 'center',
              gap: 1,
            }}
          >
            <Avatar
              sx={{
                width: 36,
                height: 36,
                backgroundColor: '#fff',
                color: '#1976d2',
                fontSize: '1.2rem',
              }}
            >
              {user?.firstName?.charAt(0) ?? 'U'}
            </Avatar>
          </IconButton>

          {/* User Dropdown Menu */}
          <Menu
            anchorEl={anchorEl}
            open={Boolean(anchorEl)}
            onClose={handleMenuClose}
            anchorOrigin={{
              vertical: 'bottom',
              horizontal: 'right',
            }}
            transformOrigin={{
              vertical: 'top',
              horizontal: 'right',
            }}
          >
            <MenuItem disabled>
              <Typography variant="body2" sx={{ fontWeight: 'bold' }}>
                {user?.email}
              </Typography>
            </MenuItem>
            <Divider />
            <MenuItem onClick={() => { navigate('/profile'); handleMenuClose(); }}>
              <AccountCircleIcon sx={{ mr: 1 }} />
              Perfil
            </MenuItem>
            <MenuItem onClick={() => { navigate('/settings'); handleMenuClose(); }}>
              <SettingsIcon sx={{ mr: 1 }} />
              Configurações
            </MenuItem>
            <Divider />
            <MenuItem onClick={handleLogout}>
              <LogoutIcon sx={{ mr: 1 }} />
              Sair
            </MenuItem>
          </Menu>
        </Toolbar>
      </AppBar>

      {/* Drawer Desktop */}
      <Box
        component="nav"
        sx={{
          width: { sm: DRAWER_WIDTH },
          flexShrink: { sm: 0 },
        }}
      >
        {/* Drawer Mobile */}
        <Drawer
          variant="temporary"
          open={mobileOpen}
          onClose={handleDrawerToggle}
          ModalProps={{
            keepMounted: true,
          }}
          sx={{
            display: { xs: 'block', sm: 'none' },
            '& .MuiDrawer-paper': {
              boxSizing: 'border-box',
              width: DRAWER_WIDTH,
            },
          }}
        >
          {drawerContent}
        </Drawer>

        {/* Drawer Desktop */}
        <Drawer
          variant="permanent"
          sx={{
            display: { xs: 'none', sm: 'block' },
            '& .MuiDrawer-paper': {
              boxSizing: 'border-box',
              width: DRAWER_WIDTH,
            },
          }}
          open
        >
          {drawerContent}
        </Drawer>
      </Box>

      {/* Main Content */}
      <Box
        component="main"
        sx={{
          flexGrow: 1,
          p: 3,
          width: { sm: `calc(100% - ${DRAWER_WIDTH}px)` },
          mt: 8,
        }}
      >
        <Outlet />
      </Box>
    </Box>
  );
};
