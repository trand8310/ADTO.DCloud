(function ($) {
    var _tenantService = adto.services.app.tenant,
        l = adto.localization.getSource('AuthServer'),
        _$modal = $('#TenantCreateModal'),
        _$form = _$modal.find('form'),
        _$table = $('#TenantsTable');

    var _$tenantsTable = _$table.DataTable({
        paging: true,
        serverSide: true,
        listAction: {
            ajaxFunction: _tenantService.getAll,
            inputFilter: function () {
                return $('#TenantsSearchForm').serializeFormToObject(true);
            }
        },
        buttons: [
            {
                name: 'refresh',
                text: '<i class="fas fa-redo-alt"></i>',
                action: () => _$tenantsTable.draw(false)
            }
        ],
        responsive: {
            details: {
                type: 'column'
            }
        },
        columnDefs: [
            {
                targets: 0,
                className: 'control',
                defaultContent: '',
            },
            {
                targets: 1,
                data: 'tenancyName',
                sortable: false
            },
            {
                targets: 2,
                data: 'name',
                sortable: false
            },
            {
                targets: 3,
                data: 'isActive',
                sortable: false,
                render: data => `<input type="checkbox" disabled ${data ? 'checked' : ''}>`
            },
            {
                targets: 4,
                data: null,
                sortable: false,
                autoWidth: false,
                defaultContent: '',
                render: (data, type, row, meta) => {
                    return [
                        `   <button type="button" class="btn btn-sm bg-secondary edit-tenant" data-tenant-id="${row.id}" data-toggle="modal" data-target="#TenantEditModal">`,
                        `       <i class="fas fa-pencil-alt"></i> ${l('Edit')}`,
                        '   </button>',
                        `   <button type="button" class="btn btn-sm bg-danger delete-tenant" data-tenant-id="${row.id}" data-tenancy-name="${row.name}">`,
                        `       <i class="fas fa-trash"></i> ${l('Delete')}`,
                        '   </button>'
                    ].join('');
                }
            }
        ]
    });

    _$form.find('.save-button').click(function (e) {
        e.preventDefault();

        if (!_$form.valid()) {
            return;
        }

        var tenant = _$form.serializeFormToObject();

        adto.ui.setBusy(_$modal);

        _tenantService
            .create(tenant)
            .done(function () {
                _$modal.modal('hide');
                _$form[0].reset();
                adto.notify.info(l('SavedSuccessfully'));
                _$tenantsTable.ajax.reload();
            })
            .always(function () {
                adto.ui.clearBusy(_$modal);
            });
    });

    $(document).on('click', '.delete-tenant', function () {
        var tenantId = $(this).attr('data-tenant-id');
        var tenancyName = $(this).attr('data-tenancy-name');

        deleteTenant(tenantId, tenancyName);
    });

    $(document).on('click', '.edit-tenant', function (e) {
        var tenantId = $(this).attr('data-tenant-id');

        adto.ajax({
            url: adto.appPath + 'Tenants/EditModal?tenantId=' + tenantId,
            type: 'POST',
            dataType: 'html',
            success: function (content) {
                $('#TenantEditModal div.modal-content').html(content);
            },
            error: function (e) {
            }
        });
    });

    adto.event.on('tenant.edited', (data) => {
        _$tenantsTable.ajax.reload();
    });

    function deleteTenant(tenantId, tenancyName) {
        adto.message.confirm(
            adto.utils.formatString(
                l('AreYouSureWantToDelete'),
                tenancyName
            ),
            null,
            (isConfirmed) => {
                if (isConfirmed) {
                    _tenantService
                        .delete({
                            id: tenantId
                        })
                        .done(() => {
                            adto.notify.info(l('SuccessfullyDeleted'));
                            _$tenantsTable.ajax.reload();
                        });
                }
            }
        );
    }

    _$modal.on('shown.bs.modal', () => {
        _$modal.find('input:not([type=hidden]):first').focus();
    }).on('hidden.bs.modal', () => {
        _$form.clearForm();
    });

    $('.btn-search').on('click', (e) => {
        _$tenantsTable.ajax.reload();
    });

    $('.btn-clear').on('click', (e) => {
        $('input[name=Keyword]').val('');
        $('input[name=IsActive][value=""]').prop('checked', true);
        _$tenantsTable.ajax.reload();
    });

    $('.txt-search').on('keypress', (e) => {
        if (e.which == 13) {
            _$tenantsTable.ajax.reload();
            return false;
        }
    });
})(jQuery);
